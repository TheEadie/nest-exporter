using System.Net;
using Microsoft.Extensions.Logging;
using NestExporter.Nest;
using NSubstitute;
using Shouldly;

namespace NestExporter.Tests.Nest;

public class ServiceShould
{
    private INestClient _nestClient;
    private IHttpClientFactory _httpClientFactory;

    [SetUp]
    public void SetUp()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<NestClient>>();

        _nestClient = new NestClient(_httpClientFactory, logger);
    }

    [Test]
    public async Task ReturnTemperatureWhenReceivedFromNest()
    {
        using var message = new MockHttpMessageHandler(
            @"{
        ""devices"": [
        {
            ""name"" : ""enterprises/project-id/devices/device-id"",
            ""traits"" : {
                ""sdm.devices.traits.Temperature"" : {
                    ""ambientTemperatureCelsius"" : 23.0
                }
            }
        }]}",
            HttpStatusCode.OK);
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);


        var result = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
        result.ActualTemp.ShouldBe(23);
    }
}