using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using nest_exporter.Nest;

namespace nest_exporter.Services
{
    internal class ThermostatCollector : IThermostatCollector
    {
        private readonly ILogger<ThermostatCollector> _logger;
        private readonly NestClient _nestClient;

        public ThermostatCollector(NestClient nestClient, ILogger<ThermostatCollector> logger)
        {
            _logger = logger;
            _nestClient = nestClient;
        }

        public async Task Monitor(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Running nest monitoring loop");

                await _nestClient.RefreshAccessToken();
                var currentTemperature = await _nestClient.GetThermostatInfo();

                _logger.LogInformation($"Central heating is {currentTemperature.Status}. " +
                                       $"It is currently {currentTemperature.ActualTemp}C, {currentTemperature.Humidity}% humidity. " +
                                       $"Target is {currentTemperature.TargetTemp}c");

                await Task.Delay(60000, cancellationToken);
            }
        }
    }
}
