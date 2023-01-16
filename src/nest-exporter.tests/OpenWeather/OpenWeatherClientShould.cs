using System.Net;
using NestExporter.OpenWeather;
using NSubstitute;
using Shouldly;

namespace NestExporter.Tests.OpenWeather;

public class OpenWeatherClientShould
{
    private IOpenWeatherClient _openWeatherClient;
    private IHttpClientFactory _httpClientFactory;

    [SetUp]
    public void SetUp()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _openWeatherClient = new OpenWeatherClient(_httpClientFactory, "api-key", 0, 0);
    }

    [Test]
    public async Task ReturnCurrentTemperatureWhenReceivedFromOpenWeather()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.OK,
            /*lang=json,strict*/
            @"{
                ""main"": {
                    ""temp"": 3.88
                }
            }");
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _openWeatherClient.GetCurrentWeather().ConfigureAwait(false);
        result.CurrentTemperature.ShouldBe(3.88);
    }

    [Test]
    public async Task ReturnCurrentFeelsLikeTemperatureWhenReceivedFromOpenWeather()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.OK,
            /*lang=json,strict*/
            @"{
                ""main"": {
                    ""feels_like"": -0.81
                }
            }");
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _openWeatherClient.GetCurrentWeather().ConfigureAwait(false);
        result.FeelsLikeTemperature.ShouldBe(-0.81);
    }

    [Test]
    public async Task ReturnCurrentHumidityWhenReceivedFromOpenWeather()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.OK,
            /*lang=json,strict*/
            @"{
                ""main"": {
                    ""humidity"": 87
                }
            }");
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _openWeatherClient.GetCurrentWeather().ConfigureAwait(false);
        result.CurrentHumidity.ShouldBe(87);
    }
}