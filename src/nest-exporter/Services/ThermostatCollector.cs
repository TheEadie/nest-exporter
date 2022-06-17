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

    private static readonly Gauge ConnectionStatus =
        Metrics.CreateGauge("nest_thermostat_connection_status", "0 if the thermostat is offline, 1 if it is online", Labels);

    private static readonly Gauge EcoMode =
        Metrics.CreateGauge("nest_thermostat_eco_mode", "0 if ECO mode is off, 1 if ECO mode is on", Labels);

    private readonly ILogger<ThermostatCollector> _logger;
    private readonly INestClientFactory _nestClientFactory;
    private readonly IConfiguration _configuration;

    public ThermostatCollector(INestClientFactory nestClientFactory, IConfiguration configuration, ILogger<ThermostatCollector> logger)
    {
        _logger = logger;
        _nestClientFactory = nestClientFactory;
        _configuration = configuration;
    }

    public async Task Monitor(CancellationToken cancellationToken)
    {
        var nestClient = _nestClientFactory.Create(_configuration["NestApi:ClientId"],
            _configuration["NestApi:ClientSecret"],
            _configuration["NestApi:ProjectId"],
            _configuration["NestApi:RefreshToken"]);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Calling Nest API to update stats");

                var thermostatInfo = await nestClient.GetThermostatInfo().ConfigureAwait(false);
                ActualTemp.WithLabels(thermostatInfo.Name).Set(thermostatInfo.ActualTemp);
                TargetTemp.WithLabels(thermostatInfo.Name).Set(thermostatInfo.TargetTemp);
                Humidity.WithLabels(thermostatInfo.Name).Set(thermostatInfo.Humidity);
                Status.WithLabels(thermostatInfo.Name).Set(thermostatInfo.HeatingStatus == HeatingStatus.Off ? 0 : 1);
                ConnectionStatus.WithLabels(thermostatInfo.Name).Set(thermostatInfo.ConnectionStatus == Nest.ConnectionStatus.Offline ? 0 : 1);
                EcoMode.WithLabels(thermostatInfo.Name).Set(thermostatInfo.EcoMode ? 1 : 0);

                _logger.LogInformation("{Name}: " +
                            "Thermostat is {ConnectionStatus}. " +
                            "Central heating is {Status}. " +
                            "Eco mode is {EcoMode}. " +
                            "It is currently {ActualTemp}C, {Humidity}% humidity. " +
                            "Target is {TargetTemp}c",
                            thermostatInfo.Name,
                            thermostatInfo.ConnectionStatus,
                            thermostatInfo.HeatingStatus,
                            thermostatInfo.EcoMode ? "ON" : "OFF",
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
