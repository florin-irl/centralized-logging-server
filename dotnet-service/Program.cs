using Serilog.Formatting.Json;
using Serilog;
using Serilog.Sinks.Network;
using dotnet_service.Data;
using Microsoft.EntityFrameworkCore;
using dotnet_service.Repositories;
using dotnet_service.Services;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("service", "dotnet-service")
    .WriteTo.File(new JsonFormatter(), "Logs/local.log", rollingInterval: RollingInterval.Day)
    .WriteTo.TCPSink("tcp://localhost:5044", new JsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddControllers();

builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.UseHttpsRedirection();

// Sample log to test
Log.Information("Dotnet service started and logging initialized.");

app.Run();
