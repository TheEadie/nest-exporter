using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NestExporter.Nest;
using Prometheus;

namespace NestExporter.Services;

internal class ThermostatCollector : IThermostatCollector
{
    private static readonly GaugeConfiguration Labels = new()
    {
        LabelNames = new[] { "name" }
    };

    private static readonly Gauge ActualTemp =
        Metrics.CreateGauge("nest_thermostat_actual_temperature", "The actual temperature in the room", Labels);

    private static readonly Gauge Humidity =
        Metrics.CreateGauge("nest_thermostat_humidity", "The humidity in the room", Labels);

    private static readonly Gauge TargetTemp =
        Metrics.CreateGauge("nest_thermostat_target_temperature", "The target temperature for the room", Labels);

    private static readonly Gauge Status =
        Metrics.CreateGauge("nest_thermostat_status", "0 if the heating is off, 1 if it is on", Labels);

    private readonly ILogger<ThermostatCollector> _logger;
    private readonly INestClient _nestClient;
    private readonly IConfiguration _configuration;

    public ThermostatCollector(INestClient nestClient, IConfiguration configuration, ILogger<ThermostatCollector> logger)
    {
        _logger = logger;
        _nestClient = nestClient;
        _configuration = configuration;
    }

    public async Task Monitor(CancellationToken cancellationToken)
    {
        _nestClient.Configure(_configuration["NestApi:ClientId"],
            _configuration["NestApi:ClientSecret"],
            _configuration["NestApi:ProjectId"],
            _configuration["NestApi:RefreshToken"]);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Calling Nest API to update stats");

                var thermostatInfo = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
                ActualTemp.WithLabels(thermostatInfo.Name).Set(thermostatInfo.ActualTemp);
                TargetTemp.WithLabels(thermostatInfo.Name).Set(thermostatInfo.TargetTemp);
                Humidity.WithLabels(thermostatInfo.Name).Set(thermostatInfo.Humidity);
                Status.WithLabels(thermostatInfo.Name).Set(thermostatInfo.Status == "OFF" ? 0 : 1);

                _logger.LogInformation("{Name}: " +
                            "Central heating is {Status}. " +
                            "It is currently {ActualTemp}C, {Humidity}% humidity. " +
                            "Target is {TargetTemp}c",
                            thermostatInfo.Name,
                            thermostatInfo.Status,
                            thermostatInfo.ActualTemp,
                            thermostatInfo.Humidity,
                            thermostatInfo.TargetTemp);

                await Task.Delay(60000, cancellationToken).ConfigureAwait(false);
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "Error calling Nest API. Couldn't deserialise response");
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Error calling Nest API. Issue calling HTTP API");
            }
        }
    }
}
