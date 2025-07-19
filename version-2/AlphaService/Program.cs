using Serilog;
using Serilog.Context;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ✅ SIMPLE TEXT LOGGING
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "AlphaService")
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Suppress noisy ASP.NET logs
    .MinimumLevel.Debug() // Keep your detailed logs
    .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter()) // JSON for console (useful if shipping via Filebeat)
    .WriteTo.File(
        new Serilog.Formatting.Json.JsonFormatter(),
        "Logs/alpha-service.json",
        rollingInterval: RollingInterval.Day
    )
    .CreateLogger();



builder.Host.UseSerilog();

// ✅ Normal app setup
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Serilog request logging middleware (optional but recommended)
app.UseSerilogRequestLogging();

// ✅ Correlation ID middleware (place it HERE)
app.Use(async (context, next) =>
{
    var correlationId = context.TraceIdentifier; // ASP.NET built-in unique request ID
    LogContext.PushProperty("CorrelationId", correlationId);
    await next();
});

app.UseAuthorization();
app.MapControllers();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
