using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using NestExporter.Nest;
using NestExporter.Services;

namespace NestExporter;

public static class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging => logging.AddConsole())
            .ConfigureAppConfiguration((_, config) => config.AddEnvironmentVariables(prefix: "NestExporter_"))
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .ConfigureServices(services =>
            {
                _ = services.AddLogging()
                    .AddHttpClient()
                    // Remove logging from httpclient as it prints every request to info
                    .RemoveAll<IHttpMessageHandlerBuilderFilter>()
                    .AddHostedService<ThermostatCollectorService>()
                    .AddScoped<IThermostatCollector, ThermostatCollector>()
                    .AddScoped<NestClient>();
            });
    }
}
