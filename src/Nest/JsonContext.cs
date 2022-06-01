using System.Text.Json.Serialization;

namespace NestExporter.Nest;

[JsonSerializable(typeof(DevicesResponse))]
[JsonSerializable(typeof(RefreshAccessTokenResponse))]
internal partial class JsonContext : JsonSerializerContext
{
}