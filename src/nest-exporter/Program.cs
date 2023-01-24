using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using NestExporter.Nest;
using NestExporter.OpenWeather;
using NestExporter.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole().AddJsonConsole();
builder.Configuration.AddEnvironmentVariables(prefix: "NestExporter_");
builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
builder.Services.AddHostedService<ThermostatCollectorService>();
builder.Services.AddScoped<IThermostatCollector, ThermostatCollector>();
builder.Services.AddScoped<INestClientFactory, NestClientFactory>();
builder.Services.AddHostedService<WeatherCollectorService>();
builder.Services.AddScoped<IWeatherCollector, WeatherCollector>();
builder.Services.AddScoped<IOpenWeatherClientFactory, OpenWeatherClientFactory>();

var app = builder.Build();

Metrics.SuppressDefaultMetrics();
app.MapMetrics();

app.MapGet("/", () => Results.Redirect("/metrics"));

app.Run();


