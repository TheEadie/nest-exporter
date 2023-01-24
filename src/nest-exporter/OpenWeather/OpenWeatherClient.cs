using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace NestExporter.OpenWeather;

public class OpenWeatherClient : IOpenWeatherClient
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _apiKey;
    private readonly double _longitude;
    private readonly double _latitude;

    public OpenWeatherClient(IHttpClientFactory clientFactory, string apiKey, double longitude, double latitude)
    {
        _clientFactory = clientFactory;
        _apiKey = apiKey;
        _longitude = longitude;
        _latitude = latitude;
    }

    public async Task<CurrentWeatherInfo> GetCurrentWeather()
    {
        var result =
            await CallOpenWeatherApi<WeatherResponse>(
                new Uri($"data/2.5/weather?lat={_latitude}&lon={_longitude}&appid={_apiKey}&units=metric",
                    UriKind.Relative)).ConfigureAwait(false);
        return GetCurrentWeatherInfo(result);
    }

    private static CurrentWeatherInfo GetCurrentWeatherInfo(WeatherResponse weather) =>
        new(weather.Current.Temperature, weather.Current.FeelsLikeTemperature, weather.Current.Humidity);

    private async Task<T> CallOpenWeatherApi<T>(Uri requestUri)
    {
        using var httpClient = _clientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://api.openweathermap.org/");

        var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var streamAsync = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        await using var stream = streamAsync.ConfigureAwait(false);
        var result = await JsonSerializer.DeserializeAsync(streamAsync, typeof(T), JsonContext.Default).ConfigureAwait(false);
        return result is null ? throw new JsonException("The API returned success but the JSON response was empty") : (T) result;
    }
}
