using Microsoft.Extensions.Hosting;
using AkkaServer.Core;
using Microsoft.AspNetCore.Hosting;
using Steeltoe.Discovery.Client;
namespace AkkaServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
              Host.CreateDefaultBuilder()
                  .UseAkkaServer("app.conf")
                  .AddDiscoveryClient()
                  .ConfigureWebHostDefaults(webBuilder => {
                      webBuilder
                      .UseStartup<Startup>();
                  })
                  .Build()
                  .Run();
         }
    }
}
