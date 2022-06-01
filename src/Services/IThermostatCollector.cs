using System.Threading;
using System.Threading.Tasks;

namespace NestExporter.Services;

internal interface IThermostatCollector
{
    Task Monitor(CancellationToken cancellationToken);
}
