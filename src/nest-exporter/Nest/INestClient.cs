using System.Threading.Tasks;

namespace NestExporter.Nest;

internal interface INestClient
{
    void Configure(string clientId, string clientSecret, string projectId, string refreshToken);
    Task<ThermostatInfo> GetThermostatInfo();
}
