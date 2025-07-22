using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

// ✅ STANDARDIZED JSON LOGGING
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "AlphaService")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console(new JsonFormatter(renderMessage: true))
    .WriteTo.File(
        new JsonFormatter(renderMessage: true),
        "Logs/alpha-service.json",
        rollingInterval: RollingInterval.Day
    )
    .WriteTo.DurableHttpUsingFileSizeRolledBuffers(
        requestUri: "http://localhost:8080",
        batchFormatter: new Serilog.Sinks.Http.BatchFormatters.ArrayBatchFormatter()
    )
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHttpClient();
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

// ✅ STANDARDIZED REQUEST LOGGING
app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diag, ctx) =>
    {
        diag.Set("Service", "AlphaService");
        diag.Set("CorrelationId", ctx.TraceIdentifier);
        diag.Set("RequestMethod", ctx.Request.Method);
        diag.Set("RequestPath", ctx.Request.Path);
    };
});

// ✅ Correlation ID middleware (after SerilogRequestLogging)
app.Use(async (context, next) =>
{
    LogContext.PushProperty("CorrelationId", context.TraceIdentifier);
    await next();
});

app.UseCors("AllowAll");


app.UseAuthorization();
app.MapControllers();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
