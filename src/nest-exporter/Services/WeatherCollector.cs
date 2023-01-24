using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NestExporter.OpenWeather;
using Prometheus;

namespace NestExporter.Services;

internal class WeatherCollector : IWeatherCollector
{
    private static readonly Gauge ActualTemp =
        Metrics.CreateGauge("nest_outside_actual_temperature", "The actual temperature outside");
    private static readonly Gauge FeelsLikeTemp =
        Metrics.CreateGauge("nest_outside_feels_like_temperature", "The temperature it feels like outside");
    private static readonly Gauge Humidity =
        Metrics.CreateGauge("nest_outside_humidity", "The humidity outside");

    private readonly ILogger<WeatherCollector> _logger;
    private readonly IOpenWeatherClientFactory _openWeatherClientFactory;
    private readonly IConfiguration _configuration;

    public WeatherCollector(IOpenWeatherClientFactory openWeatherClientFactory, IConfiguration configuration, ILogger<WeatherCollector> logger)
    {
        _logger = logger;
        _openWeatherClientFactory = openWeatherClientFactory;
        _configuration = configuration;
    }

    public async Task Monitor(CancellationToken cancellationToken)
    {
        var apiKey = _configuration["OpenWeatherApi:ApiKey"];
        var longitude = _configuration["OpenWeatherApi:Longitude"];
        var latitude = _configuration["OpenWeatherApi:Latitude"];

        _logger.LogDebug("ApiKey: {ApiKey}", _configuration["OpenWeatherApi:ApiKey"]);
        _logger.LogDebug("Longitude: {Longitude}", _configuration["OpenWeatherApi:Longitude"]);
        _logger.LogDebug("Latitude: {Latitude}", _configuration["OpenWeatherApi:Latitude"]);

        if (apiKey is null)
        {
            _logger.LogWarning(
                "API Key not configured. Please set the NestExporter_OpenWeatherApi__ApiKey environment variable");
            return;
        }

        if (longitude is null)
        {
            _logger.LogWarning(
                "Longitude not configured. Please set the NestExporter_OpenWeatherApi__Longitude environment variable");
            return;
        }

        if (latitude is null)
        {
            _logger.LogWarning(
                "Latitude not configured. Please set the NestExporter_OpenWeatherApi__Latitude environment variable");
            return;
        }

        var openWeatherClient = _openWeatherClientFactory.Create(apiKey, double.Parse(longitude), double.Parse(latitude));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Calling Open Weather API to update stats");

                var weather = await openWeatherClient.GetCurrentWeather().ConfigureAwait(false);

                ActualTemp.Set(weather.CurrentTemperature);
                FeelsLikeTemp.Set(weather.FeelsLikeTemperature);
                Humidity.Set(weather.CurrentHumidity);

                _logger.LogInformation("Outside: " +
                                           "It is currently {Temperature}C (Feels like {FeelsLikeTemperature}), {Humidity}% humidity. ",
                        weather.CurrentTemperature,
                        weather.FeelsLikeTemperature,
                        weather.CurrentHumidity);
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "Error calling Open Weather API. Couldn't deserialise response");
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Error calling Open Weather API. Issue calling HTTP API");
            }

            await Task.Delay(60000, cancellationToken).ConfigureAwait(false);
        }
    }
}
