using System.Net;
using System.Text.Json;
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
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.OK,
            @"{
        ""devices"": [
        {
            ""name"" : ""enterprises/project-id/devices/device-id"",
            ""traits"": {
                ""sdm.devices.traits.Info"" : {
                    ""customName"" : ""My device""
                }
            }
        }]}");
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
        result.Name.ShouldBe("My device");
    }

    [Test]
    public async Task ReturnTemperatureWhenReceivedFromNest()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.OK,
            @"{
        ""devices"": [
        {
            ""name"" : ""enterprises/project-id/devices/device-id"",
            ""traits"" : {
                ""sdm.devices.traits.Temperature"" : {
                    ""ambientTemperatureCelsius"" : 23.0
                }
            }
        }]}");
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
        result.ActualTemp.ShouldBe(23);
    }

    [Test]
    public async Task ReturnHumidityWhenReceivedFromNest()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.OK,
            @"{
        ""devices"": [
        {
            ""name"" : ""enterprises/project-id/devices/device-id"",
            ""traits"" : {
                ""sdm.devices.traits.Humidity"" : {
                    ""ambientHumidityPercent"" : 35.0
                }
            }
        }]}");
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
        result.Humidity.ShouldBe(35);
    }

    [Test]
    public async Task ReturnTargetTemperatureWhenReceivedFromNest()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.OK,
            @"{
        ""devices"": [
        {
            ""name"" : ""enterprises/project-id/devices/device-id"",
            ""traits"" : {
                ""sdm.devices.traits.ThermostatTemperatureSetpoint"" : {
                    ""heatCelsius"" : 23.0
                }
            }
        }]}");
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
        result.TargetTemp.ShouldBe(23);
    }

    [Test]
    public async Task ReturnStatusWhenReceivedFromNest()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.OK,
            @"{
        ""devices"": [
        {
            ""name"" : ""enterprises/project-id/devices/device-id"",
            ""traits"" : {
                ""sdm.devices.traits.ThermostatHvac"" : {
                    ""status"" : ""HEATING""
                }
            }
        }]}");
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var result = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
        result.Status.ShouldBe("HEATING");
    }

    [Test]
    public async Task ThrowExceptionWhenNestApiErrors()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.InternalServerError, string.Empty);
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        _ = await Should.ThrowAsync<HttpRequestException>(async () =>
            await _nestClient.GetThermostatInfo().ConfigureAwait(false)).ConfigureAwait(false);
    }

    [Test]
    public async Task ThrowJsonExceptionWhenResponseCantBeDeserialized()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.OK, "This is not JSON");
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        _ = await Should.ThrowAsync<JsonException>(async () =>
            await _nestClient.GetThermostatInfo().ConfigureAwait(false)).ConfigureAwait(false);
    }

    [Test]
    public async Task AuthenticateWhenAccessTokenHasExpired()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.Unauthorized, string.Empty);
        message.AddResponse(HttpStatusCode.OK,
            @"{
        ""devices"": [
        {
            ""name"" : ""enterprises/project-id/devices/device-id"",
            ""traits"" : {
                ""sdm.devices.traits.Temperature"" : {
                    ""ambientTemperatureCelsius"" : 23.0
                }
            }
        }]}");

        using var authMessage = new MockHttpMessageHandler();
        authMessage.AddResponse(HttpStatusCode.OK, @"{""access_token"": ""this-is-an-access-token""}");

        using var httpClient = new HttpClient(message);
        using var authHttpClient = new HttpClient(authMessage);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient, authHttpClient);

        var result = await _nestClient.GetThermostatInfo().ConfigureAwait(false);
        result.ActualTemp.ShouldBe(23);
    }

    [Test]
    public async Task ThrowExceptionWhenGoogleAuthApiErrors()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.Unauthorized, string.Empty);

        using var authMessage = new MockHttpMessageHandler();
        authMessage.AddResponse(HttpStatusCode.InternalServerError, string.Empty);

        using var httpClient = new HttpClient(message);
        using var authHttpClient = new HttpClient(authMessage);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient, authHttpClient);

        _ = await Should.ThrowAsync<HttpRequestException>(async () =>
            await _nestClient.GetThermostatInfo().ConfigureAwait(false)).ConfigureAwait(false);
    }

    [Test]
    public async Task ThrowJsonExceptionWhenAuthResponseCantBeDeserialized()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.Unauthorized, string.Empty);

        using var authMessage = new MockHttpMessageHandler();
        authMessage.AddResponse(HttpStatusCode.OK, "This is not JSON");

        using var httpClient = new HttpClient(message);
        using var authHttpClient = new HttpClient(authMessage);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient, authHttpClient);

        _ = await Should.ThrowAsync<JsonException>(async () =>
            await _nestClient.GetThermostatInfo().ConfigureAwait(false)).ConfigureAwait(false);
    }

    [Test]
    public async Task ThrowExceptionWhenNestApiErrorsAfterAccessTokenRefresh()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.Unauthorized, string.Empty);
        message.AddResponse(HttpStatusCode.InternalServerError, string.Empty);

        using var authMessage = new MockHttpMessageHandler();
        authMessage.AddResponse(HttpStatusCode.OK, @"{""access_token"": ""this-is-an-access-token""}");

        using var httpClient = new HttpClient(message);
        using var authHttpClient = new HttpClient(authMessage);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient, authHttpClient);

        _ = await Should.ThrowAsync<HttpRequestException>(async () =>
            await _nestClient.GetThermostatInfo().ConfigureAwait(false)).ConfigureAwait(false);
    }

    [Test]
    public async Task ThrowExceptionWhenResponseCantBeDeserializedAfterAccessTokenRefresh()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.Unauthorized, string.Empty);
        message.AddResponse(HttpStatusCode.OK, "This is not JSON");

        using var authMessage = new MockHttpMessageHandler();
        authMessage.AddResponse(HttpStatusCode.OK, @"{""access_token"": ""this-is-an-access-token""}");

        using var httpClient = new HttpClient(message);
        using var authHttpClient = new HttpClient(authMessage);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient, authHttpClient);

        _ = await Should.ThrowAsync<JsonException>(async () =>
            await _nestClient.GetThermostatInfo().ConfigureAwait(false)).ConfigureAwait(false);
    }
}
