using System.Net.Http;

namespace NestExporter.OpenWeather;

internal class OpenWeatherClientFactory : IOpenWeatherClientFactory
{
    private readonly IHttpClientFactory _clientFactory;

    public OpenWeatherClientFactory(IHttpClientFactory clientFactory)
        => _clientFactory = clientFactory;

    public IOpenWeatherClient Create(string apiKey, double longitude, double latitude)
        => new OpenWeatherClient(_clientFactory, apiKey, longitude, latitude);
}
