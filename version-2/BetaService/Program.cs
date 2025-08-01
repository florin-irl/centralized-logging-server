using Serilog;
using Serilog.Context;
using Serilog.Events;
// This is the formatter, which is correct
// This package provides the .TCPSink() method
using BetaService.Data;
using Microsoft.EntityFrameworkCore;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Network;
using Elastic.CommonSchema.Serilog;
using Elastic.Apm.NetCoreAll;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAllElasticApm();

// Final Logger Configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(new EcsTextFormatter())
    .WriteTo.File(new EcsTextFormatter(), "Logs/beta-service-ecs.json", rollingInterval: RollingInterval.Day)
    // --> Using the TCPSink from your chosen package, with the ECS formatter
    .WriteTo.TCPSink(
        "tcp://localhost:5000",
        textFormatter: new EcsTextFormatter()
    )
    .CreateLogger();

builder.Host.UseSerilog();

// --- The rest of your file remains the same ---

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(opts => { opts.JsonSerializerOptions.PropertyNamingPolicy = null; });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("X-Trace-Id", out var traceId))
    {
        LogContext.PushProperty("trace.id", traceId.ToString());
    }
    else
    {
        LogContext.PushProperty("trace.id", context.TraceIdentifier);
    }
    await next();
});

app.UseAuthorization();
app.MapControllers();
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();