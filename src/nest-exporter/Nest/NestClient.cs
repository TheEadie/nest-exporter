using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NestExporter.Nest;

internal class NestClient : INestClient
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<NestClient> _logger;

    private string _clientId = "";
    private string _clientSecret = "";
    private string _projectId = "";

    private string _refreshToken = "";
    private string _accessToken = "";

    public NestClient(IHttpClientFactory clientFactory, ILogger<NestClient> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public void Configure(string clientId, string clientSecret, string projectId, string refreshToken)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _projectId = projectId;
        _refreshToken = refreshToken;
    }

    public async Task<ThermostatInfo> GetThermostatInfo()
    {
        var result = await CallNestApi<DevicesResponse>(new Uri($"v1/enterprises/{_projectId}/devices", UriKind.Relative))
                        .ConfigureAwait(false);

        var thermostat = result.Devices.First();
        return new ThermostatInfo(thermostat.Traits.Info.Name,
            thermostat.Traits.Temperature.ActualTemperatureCelsius,
            thermostat.Traits.TargetTemperature.TargetTemperatureCelsius,
            thermostat.Traits.Humidity.HumidityPercent,
            thermostat.Traits.Hvac.Status);
    }

    private async Task<T> CallNestApi<T>(Uri requestUri)
    {
        using var httpClient = _clientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://smartdevicemanagement.googleapis.com/");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Retry with newer access token
            await RefreshAccessToken().ConfigureAwait(false);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);
        }

        _ = response.EnsureSuccessStatusCode();

        var streamAsync = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        await using var stream = streamAsync.ConfigureAwait(false);
        var result = await JsonSerializer.DeserializeAsync(streamAsync, typeof(T), JsonContext.Default).ConfigureAwait(false);
        return result is null ? throw new JsonException("The API returned success but the JSON response was empty") : (T) result;
    }

    private async Task RefreshAccessToken()
    {
        using var httpClient = _clientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://www.googleapis.com/");

        var response = await httpClient.PostAsync(new Uri(
            "oauth2/v4/token?" +
            $"client_id={_clientId}&" +
            $"client_secret={_clientSecret}&" +
            $"refresh_token={_refreshToken}&" +
            "grant_type=refresh_token",
            UriKind.Relative),
            null)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
        var streamAsync = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        await using var stream = streamAsync.ConfigureAwait(false);
        var result = await JsonSerializer.DeserializeAsync(streamAsync, JsonContext.Default.RefreshAccessTokenResponse).ConfigureAwait(false);
        if (result is null)
        {
            throw new JsonException("The API returned success but the JSON response was empty");
        }
        _accessToken = result.AccessToken;
    }
}