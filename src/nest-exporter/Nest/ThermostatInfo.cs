namespace NestExporter.Nest;

public class ThermostatInfo
{
    public string Name { get; }
    public double ActualTemp { get; }
    public double TargetTemp { get; }
    public double Humidity { get; }
    public string Status { get; }
    public string ConnectionStatus { get; }

    public ThermostatInfo(string name, double actualTemp, double targetTemp, double humidity, string status, string connectionStatus)
    {
        Name = name;
        ActualTemp = actualTemp;
        TargetTemp = targetTemp;
        Humidity = humidity;
        Status = status;
        ConnectionStatus = connectionStatus;
    }
}
