using System.Threading;
using System.Threading.Tasks;

namespace NestExporter.Services;

internal interface IWeatherCollector
{
    Task Monitor(CancellationToken cancellationToken);
}
