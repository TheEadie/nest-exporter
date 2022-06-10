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
    public async Task ReturnNameWhenReceivedFromNest()
    {
        using var message = new MockHttpMessageHandler(
            @"{
        ""devices"": [
        {
            ""name"" : ""enterprises/project-id/devices/device-id"",
            ""traits"": {
                ""sdm.devices.traits.Info"" : {
                    ""customName"" : ""My device""
                }
            }
        }]}",
            HttpStatusCode.OK);
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
        result.Name.ShouldBe("My device");
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

    [Test]
    public async Task ReturnHumidityWhenReceivedFromNest()
    {
        using var message = new MockHttpMessageHandler(
            @"{
        ""devices"": [
        {
            ""name"" : ""enterprises/project-id/devices/device-id"",
            ""traits"" : {
                ""sdm.devices.traits.Humidity"" : {
                    ""ambientHumidityPercent"" : 35.0
                }
            }
        }]}",
            HttpStatusCode.OK);
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
        result.Humidity.ShouldBe(35);
    }

    [Test]
    public async Task ReturnTargetTemperatureWhenReceivedFromNest()
    {
        using var message = new MockHttpMessageHandler(
            @"{
        ""devices"": [
        {
            ""name"" : ""enterprises/project-id/devices/device-id"",
            ""traits"" : {
                ""sdm.devices.traits.ThermostatTemperatureSetpoint"" : {
                    ""heatCelsius"" : 23.0
                }
            }
        }]}",
            HttpStatusCode.OK);
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
        result.TargetTemp.ShouldBe(23);
    }

    [Test]
    public async Task ReturnStatusWhenReceivedFromNest()
    {
        using var message = new MockHttpMessageHandler(
            @"{
        ""devices"": [
        {
            ""name"" : ""enterprises/project-id/devices/device-id"",
            ""traits"" : {
                ""sdm.devices.traits.ThermostatHvac"" : {
                    ""status"" : ""HEATING""
                }
            }
        }]}",
            HttpStatusCode.OK);
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
        result.Status.ShouldBe("HEATING");
    }
}