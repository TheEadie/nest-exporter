// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// Disable Resharper naming for this file - Used to deserialize from Google API

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace nest_exporter.Nest;

internal class DevicesResponse
{
    [JsonPropertyName("devices")]
    public IEnumerable<DeviceResponse> Devices { get; set; }
}

internal class DeviceResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("assignee")]
    public string Assignee { get; set; }

    [JsonPropertyName("traits")]
    public TraitResponse Traits { get; set; }
}

internal class TraitResponse
{
    [JsonPropertyName("sdm.devices.traits.Info")]
    public TraitInfo Info { get; set; }

    [JsonPropertyName("sdm.devices.traits.Temperature")]
    public TraitTemperature Temperature { get; set; }

    [JsonPropertyName("sdm.devices.traits.Humidity")]
    public TraitHumidity Humidity { get; set; }

    [JsonPropertyName("sdm.devices.traits.ThermostatTemperatureSetpoint")]
    public TraitTargetTemperature TargetTemperature { get; set; }

    [JsonPropertyName("sdm.devices.traits.ThermostatHvac")]
    public TraitHavc Hvac { get; set; }
}

internal class TraitInfo
{
    [JsonPropertyName("customName")]
    public string Name { get; set; }
}

internal class TraitHavc
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
}

internal class TraitHumidity
{
    [JsonPropertyName("ambientHumidityPercent")]
    public double HumidityPercent { get; set; }
}

internal class TraitTargetTemperature
{
    [JsonPropertyName("heatCelsius")]
    public double TargetTemperatureCelsius { get; set; }
}

internal class TraitTemperature
{
    [JsonPropertyName("ambientTemperatureCelsius")]
    public double ActualTemperatureCelsius { get; set; }
}
