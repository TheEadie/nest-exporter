using System.Collections.Generic;
using System.Threading.Tasks;

namespace NestExporter.Nest;

internal interface INestClient
{
    Task<IReadOnlyCollection<ThermostatInfo>> GetThermostatInfo();
}
