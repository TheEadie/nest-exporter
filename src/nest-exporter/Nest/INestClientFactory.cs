namespace NestExporter.Nest;

internal interface INestClientFactory
{
    INestClient Create(string clientId, string clientSecret, string projectId, string refreshToken);
}
