using AlphaService.Helpers;
using Elastic.Apm.NetCoreAll;
using Elastic.CommonSchema.Serilog;
using Serilog;
using Serilog.Context;
using Serilog.Events;
// --> 1. ADD a using statement for the ECS formatter.
using Serilog.Formatting.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAllElasticApm();

// --> 2. REPLACE the logger configuration with the new ECS-compliant setup.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    // Console sink now uses the ECS formatter
    .WriteTo.Console(new EcsTextFormatter())
    // File sink now uses the ECS formatter
    .WriteTo.File(
        new EcsTextFormatter(),
        "Logs/alpha-service-ecs.json", // New filename to reflect the new format
        rollingInterval: RollingInterval.Day
    )
    // HTTP sink now uses the ECS formatter.
    // The standard .Http() sink is simpler to configure with a formatter.
    .WriteTo.Http(
        "http://localhost:8080",
        queueLimitBytes: null, // <-- The fix is here
        textFormatter: new EcsTextFormatter()
    )
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TraceIdDelegatingHandler>();


builder.Services.AddHttpClient("DefaultClient")
    .AddHttpMessageHandler<TraceIdDelegatingHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --> 3. SIMPLIFY the request logger. The ECS formatter handles the details automatically.
app.UseSerilogRequestLogging();

// --> 4. REPLACE the CorrelationId middleware with the new TraceId middleware.
app.Use(async (context, next) =>
{
    string traceId;
    // Try to get the trace ID from the NGINX header first.
    if (context.Request.Headers.TryGetValue("X-Trace-Id", out var headerTraceId))
    {
        traceId = headerTraceId.ToString();
    }
    else
    {
        // Fallback to the default TraceIdentifier if the header is not present.
        traceId = context.TraceIdentifier;
    }

    // --> THE FIX, PART 1: Store the ID in HttpContext.Items
    context.Items["TraceId"] = traceId;

    // Push to Serilog's LogContext so all *local* logs have the correct ID.
    using (LogContext.PushProperty("trace.id", traceId))
    {
        await next.Invoke();
    }
});


app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();