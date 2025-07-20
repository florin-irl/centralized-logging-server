using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Json;
using BetaService.Data;
using Microsoft.EntityFrameworkCore;
using Serilog.Sinks.Network;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "BetaService")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console(new JsonFormatter(renderMessage: true))
    .WriteTo.File(
        new JsonFormatter(renderMessage: true),
        "Logs/beta-service.json",
        rollingInterval: RollingInterval.Day
    )
    .WriteTo.TCPSink("tcp://localhost:5000")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // ✅ FORCE PascalCase like Alpha
        opts.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diag, ctx) =>
    {
        diag.Set("Service", "BetaService");
        diag.Set("CorrelationId", ctx.TraceIdentifier);
        diag.Set("RequestMethod", ctx.Request.Method);
        diag.Set("RequestPath", ctx.Request.Path);
    };
});

app.Use(async (context, next) =>
{
    LogContext.PushProperty("CorrelationId", context.TraceIdentifier);
    await next();
});

app.UseAuthorization();
app.MapControllers();
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
