using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using NestExporter.Services;
using NSubstitute;
using Shouldly;

namespace NestExporter.Tests.Endpoints;

public class OpenWeatherMetricsShould
{
    private IHttpClientFactory _httpClientFactory;
    private WebApplicationFactory<Program> _application;

    [SetUp]
    public void SetUp()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();

        using var webApplicationFactory = new WebApplicationFactory<Program>();
        _application = webApplicationFactory
            .WithWebHostBuilder(builder =>
            {
                _ = builder.ConfigureServices(services =>
                {
                    _ = services.AddScoped(_ => _httpClientFactory);
                    _ = services.AddLogging(logging => logging.ClearProviders());
                    _ = services.Remove(services.SingleOrDefault(
                        d => d.ImplementationType ==
                             typeof(ThermostatCollectorService)));
                });
            });

        Environment.SetEnvironmentVariable("NestExporter_OpenWeatherApi__ApiKey", "fake-api-key");
        Environment.SetEnvironmentVariable("NestExporter_OpenWeatherApi__Longitude", "0");
        Environment.SetEnvironmentVariable("NestExporter_OpenWeatherApi__Latitude", "0");
    }

    [Test]
    public async Task DisplayAllMetrics()
    {
        using var message = new MockHttpMessageHandler();
        message.AddResponse(HttpStatusCode.OK,
        /*lang=json,strict*/
        @"{
            ""main"": {
            ""temp"": 3.88,
            ""feels_like"": -0.81,
            ""temp_min"": 2.72,
            ""temp_max"": 4.62,
            ""pressure"": 985,
            ""humidity"": 87
        }}");
        using var httpClient = new HttpClient(message);
        _ = _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        var client = _application.CreateClient();
        var response = await client.GetAsync(new Uri("/metrics", UriKind.Relative)).ConfigureAwait(false);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        content.ShouldContain(@"nest_outside_actual_temperature 3.88");
        content.ShouldContain(@"nest_outside_humidity 87");
        content.ShouldContain(@"nest_outside_feels_like_temperature -0.81");
    }
}
