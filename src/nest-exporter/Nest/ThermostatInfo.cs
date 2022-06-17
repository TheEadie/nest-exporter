namespace NestExporter.Nest;

public record ThermostatInfo(
    string Name,
    double ActualTemp,
    double TargetTemp,
    double Humidity,
    HeatingStatus HeatingStatus,
    bool EcoMode,
    ConnectionStatus ConnectionStatus);

public enum HeatingStatus
{
    Off,
    Heating
};

public enum ConnectionStatus
{
    Offline,
    Online
};
