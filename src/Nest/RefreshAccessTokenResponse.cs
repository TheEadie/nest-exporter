// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// Disable Resharper naming for this file - Used to deserialize from Google API

using System.Text.Json.Serialization;

namespace NestExporter.Nest;

internal class RefreshAccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}
