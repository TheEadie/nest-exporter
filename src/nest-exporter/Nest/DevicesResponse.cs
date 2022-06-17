// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// Disable Resharper naming for this file - Used to deserialize from Google API

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NestExporter.Nest;

internal class DevicesResponse
{
    [JsonPropertyName("devices")]
    public IEnumerable<DeviceResponse> Devices { get; set; } = new List<DeviceResponse>(0);
}

internal class DeviceResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("assignee")]
    public string Assignee { get; set; } = string.Empty;

    [JsonPropertyName("traits")]
    public TraitResponse Traits { get; set; } = new();
}

internal class TraitResponse
{
    [JsonPropertyName("sdm.devices.traits.Info")]
    public TraitInfo Info { get; set; } = new();

    [JsonPropertyName("sdm.devices.traits.Temperature")]
    public TraitTemperature Temperature { get; set; } = new();

    [JsonPropertyName("sdm.devices.traits.Humidity")]
    public TraitHumidity Humidity { get; set; } = new();

    [JsonPropertyName("sdm.devices.traits.ThermostatTemperatureSetpoint")]
    public TraitTargetTemperature TargetTemperature { get; set; } = new();

    [JsonPropertyName("sdm.devices.traits.ThermostatHvac")]
    public TraitHavc Hvac { get; set; } = new();

    [JsonPropertyName("sdm.devices.traits.Connectivity")]
    public TraitConnectivity Connectivity { get; set; } = new();

    [JsonPropertyName("sdm.devices.traits.ThermostatMode")]
    public TraitThermostatMode ThermostatMode { get; set; } = new();

    [JsonPropertyName("sdm.devices.traits.ThermostatEco")]
    public TraitEco Eco { get; set; } = new();
}

internal class TraitConnectivity
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

internal class TraitThermostatMode
{
    [JsonPropertyName("mode")]
    public string Mode { get; set; } = string.Empty;
}

internal class TraitEco
{
    [JsonPropertyName("mode")]
    public string Mode { get; set; } = string.Empty;

    [JsonPropertyName("heatCelsius")]
    public double TargetTemperatureCelsius { get; set; }
}

internal class TraitInfo
{
    [JsonPropertyName("customName")]
    public string Name { get; set; } = string.Empty;
}

internal class TraitHavc
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
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
