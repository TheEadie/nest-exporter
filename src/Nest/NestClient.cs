using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace nest_exporter.Nest
{
    public class NestClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<NestClient> _logger;

        private string _clientId = "";
        private string _clientSecret = "";
        private string _projectId = "";

        private string _refreshToken = "";
        private string _accessToken;

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
            var result = await CallNestApi<DevicesResponse>($"v1/enterprises/{_projectId}/devices");

            var thermostat = result.Devices.First();
            return new ThermostatInfo("Living Room",
                thermostat.Traits.Temperature.ActualTemperatureCelsius,
                thermostat.Traits.TargetTemperature.TargetTemperatureCelsius,
                thermostat.Traits.Humidity.HumidityPercent,
                thermostat.Traits.Hvac.Status);
        }

        private async Task<T> CallNestApi<T>(string requestUri)
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://smartdevicemanagement.googleapis.com/");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await httpClient.GetAsync(requestUri);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Retry with newer access token
                await RefreshAccessToken();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                response = await httpClient.GetAsync(requestUri);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get info from Nest API. Response: {response.StatusCode}");
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<T>(stream);
            return result;
        }

        private async Task RefreshAccessToken()
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://www.googleapis.com/");

            var response = await httpClient.PostAsync(
                "oauth2/v4/token?" +
                $"client_id={_clientId}&" +
                $"client_secret={_clientSecret}&" +
                $"refresh_token={_refreshToken}&" +
                $"grant_type=refresh_token",
                null);

            if (response.IsSuccessStatusCode)
            {
                await using var stream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<RefreshAccessTokenResponse>(stream);
                _accessToken = result.AccessToken;
            }
            else
            {
                _logger.LogError("Failed to update access token");
            }
        }
    }
}
