using System.Threading.Tasks;

namespace NestExporter.OpenWeather;

public interface IOpenWeatherClient
{
    Task<CurrentWeatherInfo> GetCurrentWeather();
}
