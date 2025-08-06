using firstcrud;
using firstcrud.Controllers.Data;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(x =>
    {
        x.Endpoint = "http://localhost:5341/ingest/otlp/v1/logs";
        x.Protocol = OtlpProtocol.HttpProtobuf;
        x.Headers = new Dictionary<string, string>
        {
            ["X-Seq-ApiKey"] = "fQaVhBQeA5UiEJECschd"
        };
        x.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = "firstcrud",
        };
    })
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

/*
builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(x =>
{
    x.SetResourceBuilder(ResourceBuilder.CreateEmpty()
        .AddService("firstcrud", "firstcrud")
        .AddAttributes(new Dictionary<string, object>()
        {
            ["deployment.environment"] = builder.Environment.EnvironmentName,
        })
    );
    x.IncludeScopes = true;
    x.IncludeFormattedMessage = true;
    x.AddOtlpExporter(a =>
    {
        a.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
        a.Protocol = OtlpExportProtocol.HttpProtobuf;
        a.Headers = "X-Seq-ApiKey=fQaVhBQeA5UiEJECschd";
    });
});
*/

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContextPool<ProductDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContextPool<UserDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}   
app.UseCors();

app.UseHttpsRedirection();
app.MapControllers();

/*var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");*/
app.Run();

/*record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}*/