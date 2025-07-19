using BetaService.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Sinks.Network;

var builder = WebApplication.CreateBuilder(args);

// ? SERILOG CONFIGURATION (Consistent with AlphaService)
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "BetaService") // Distinguish from Alpha
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Suppress noisy EF Core & ASP.NET logs
    .MinimumLevel.Debug() // Keep CRUD-level logs
    .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter()) // JSON-friendly for ELK
    .WriteTo.File(
        new Serilog.Formatting.Json.JsonFormatter(),
        "Logs/beta-service.json",
        rollingInterval: RollingInterval.Day
    )
    .WriteTo.TCPSink("tcp://localhost:5000")
    .CreateLogger();

builder.Host.UseSerilog();

// ? DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ? CONTROLLERS & SWAGGER
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

// ? REQUEST LOGGING & CORRELATION ID (Optional but recommended)
app.UseSerilogRequestLogging();
app.Use(async (context, next) =>
{
    var correlationId = context.TraceIdentifier;
    LogContext.PushProperty("CorrelationId", correlationId);
    await next();
});

app.UseAuthorization();
app.MapControllers();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);
app.Run();
