using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace nest_exporter
{
    internal class ThermostatCollector : IThermostatCollector
    {
        private readonly ILogger<ThermostatCollector> _logger;
        private int _executionCount = 0;

        public ThermostatCollector(ILogger<ThermostatCollector> logger)
        {
            _logger = logger;
        }

        public async Task Monitor(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _executionCount++;

                _logger.LogInformation(
                    "Scoped Processing Service is working. Count: {Count}", _executionCount);

                await Task.Delay(10000, cancellationToken);
            }
        }
    }
}
