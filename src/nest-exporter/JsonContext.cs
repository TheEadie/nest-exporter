using System.Text.Json.Serialization;
using NestExporter.Nest;
using NestExporter.OpenWeather;

namespace NestExporter;

[JsonSerializable(typeof(DevicesResponse))]
[JsonSerializable(typeof(RefreshAccessTokenResponse))]
[JsonSerializable(typeof(WeatherResponse))]
internal partial class JsonContext : JsonSerializerContext
{
}
