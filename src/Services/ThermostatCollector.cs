using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nest_exporter.Nest;

namespace nest_exporter.Services
{
    internal class ThermostatCollector : IThermostatCollector
    {
        private readonly ILogger<ThermostatCollector> _logger;
        private readonly NestClient _nestClient;
        private readonly IConfiguration _configuration;

        public ThermostatCollector(NestClient nestClient, IConfiguration configuration, ILogger<ThermostatCollector> logger)
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
                _logger.LogDebug("Running nest monitoring loop");

                var currentTemperature = await _nestClient.GetThermostatInfo();

                _logger.LogInformation($"Central heating is {currentTemperature.Status}. " +
                                       $"It is currently {currentTemperature.ActualTemp}C, {currentTemperature.Humidity}% humidity. " +
                                       $"Target is {currentTemperature.TargetTemp}c");

                await Task.Delay(60000, cancellationToken);
            }
        }
    }
}
