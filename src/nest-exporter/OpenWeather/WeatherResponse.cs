using System.Text.Json.Serialization;

namespace NestExporter.OpenWeather;

internal class WeatherResponse
{
    [JsonPropertyName("main")]
    public CurrentResponse Current { get; set; } = new();
}

internal class CurrentResponse
{
    [JsonPropertyName("temp")]
    public double Temperature { get; set; } = 0;

    [JsonPropertyName("feels_like")]
    public double FeelsLikeTemperature { get; set; } = 0;
    
    [JsonPropertyName("humidity")]
    public double Humidity { get; set; } = 0;
}