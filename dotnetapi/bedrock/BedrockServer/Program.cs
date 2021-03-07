using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Connections;

namespace BedrockServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
            
               .ConfigureWebHost(config =>
               {
                   config.UseKestrel(k =>
                   {
                       k.ListenLocalhost(8087, builder =>
                       {
                          builder.UseConnectionHandler<PrintConnectionHandler>();
                       });

                   }).UseStartup<Startup>()
                     .UseEnvironment(Environments.Development);

               }).ConfigureLogging(builder =>
               {
                   builder.SetMinimumLevel(LogLevel.Debug);
                   builder.AddConsole();
               });
    }
}
