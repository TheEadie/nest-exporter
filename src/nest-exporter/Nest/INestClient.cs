using System.Threading.Tasks;

namespace NestExporter.Nest;

internal interface INestClient
{
    Task<ThermostatInfo> GetThermostatInfo();
}
