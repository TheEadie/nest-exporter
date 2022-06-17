using System.Net.Http;

namespace NestExporter.Nest;

internal class NestClientFactory : INestClientFactory
{
    private readonly IHttpClientFactory _clientFactory;

    public NestClientFactory(IHttpClientFactory clientFactory)
        => _clientFactory = clientFactory;

    public INestClient Create(string clientId, string clientSecret, string projectId, string refreshToken)
        => new NestClient(_clientFactory, clientId, clientSecret, projectId, refreshToken);
}
