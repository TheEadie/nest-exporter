namespace NestExporter.Nest;

public record ThermostatInfo(
    string Name,
    double ActualTemp,
    double TargetTemp,
    double Humidity,
    string HeatingStatus,
    bool EcoMode,
    string ConnectionStatus);
