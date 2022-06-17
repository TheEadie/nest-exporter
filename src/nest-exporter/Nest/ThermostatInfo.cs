namespace NestExporter.Nest;

public record ThermostatInfo(
    string Name,
    double ActualTemp,
    double TargetTemp,
    double Humidity,
    string HeatingStatus,
    string RequestedMode,
    bool EcoMode,
    string ConnectionStatus);
