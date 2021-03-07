using Microsoft.Extensions.Hosting;
using System;
using AkkaServer.Core;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
namespace AkkaPersistenceConsole
{
    class Program
    {
        static Task Main(string[] args)
        {
            return CreateHostBuilder().Build().RunAsync();
        }

        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                       .UseAkkaServer("App.conf")
                       .ConfigureServices(services => {
                           services.AddHostedService<Worker>();
                       })
                       .ConfigureLogging(logger => {
                           logger.AddConsole();
                       })
                       .UseConsoleLifetime();
        }
    }
}
