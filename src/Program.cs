using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using nest_exporter.Nest;
using nest_exporter.Services;

namespace nest_exporter
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                    services.AddLogging();
                    services.AddHttpClient();
                    // Remove logging from httpclient as it prints every request to info
                    services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
                    services.AddHostedService<ThermostatCollectorService>();
                    services.AddScoped<IThermostatCollector, ThermostatCollector>();
                    services.AddScoped<NestClient>();
                });
    }
}
