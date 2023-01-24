namespace NestExporter.OpenWeather;

public interface IOpenWeatherClientFactory
{
    IOpenWeatherClient Create(string apiKey, double longitude, double latitude);
}
