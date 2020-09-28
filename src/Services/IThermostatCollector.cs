using System.Threading;
using System.Threading.Tasks;

namespace nest_exporter.Services
{
    internal interface IThermostatCollector
    {
        Task Monitor(CancellationToken cancellationToken);
    }
}
