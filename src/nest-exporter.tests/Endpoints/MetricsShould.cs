using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using Shouldly;

namespace NestExporter.Tests.Endpoints;

public class MetricsShould
{
    private IHttpClientFactory _httpClientFactory;
    private WebApplicationFactory<Startup> _application;

    [SetUp]
    public void SetUp()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();

        using var webApplicationFactory = new WebApplicationFactory<Startup>();
        _application = webApplicationFactory
            .WithWebHostBuilder(builder =>
            {
                _ = builder.ConfigureServices(services =>
                {
                    _ = services.AddScoped(_ => _httpClientFactory);
                });
            });
    }

    [Test]
    public async Task DisplayAllMetrics()
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
                },
                ""sdm.devices.traits.Temperature"" : {
                    ""ambientTemperatureCelsius"" : 23.0
                },
                ""sdm.devices.traits.Humidity"" : {
                    ""ambientHumidityPercent"" : 35.0
                },
                ""sdm.devices.traits.ThermostatTemperatureSetpoint"" : {
                    ""heatCelsius"" : 23.0
                },
                ""sdm.devices.traits.ThermostatHvac"" : {
                    ""status"" : ""HEATING""
                }
            }
        }]}");
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var client = _application.CreateClient();
        var response = await client.GetAsync(new Uri("/metrics", UriKind.Relative)).ConfigureAwait(false);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        content.ShouldContain(@"nest_thermostat_actual_temperature{name=""My device""} 23");
        content.ShouldContain(@"nest_thermostat_humidity{name=""My device""} 35");
        content.ShouldContain(@"nest_thermostat_target_temperature{name=""My device""} 23");
        content.ShouldContain(@"nest_thermostat_status{name=""My device""} 1");
    }
}