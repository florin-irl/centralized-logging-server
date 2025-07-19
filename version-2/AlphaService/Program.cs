using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ✅ SIMPLE TEXT LOGGING
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ServiceName", "AlphaService")
    .WriteTo.File(
        "Logs/alpha-service.log",     // Log file path
        rollingInterval: RollingInterval.Day, // New file daily
        retainedFileCountLimit: 7,    // Keep last 7 days
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.Console() // Optional: see logs in console
    .MinimumLevel.Debug() // Log everything from Debug and above
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
app.UseAuthorization();
app.MapControllers();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
