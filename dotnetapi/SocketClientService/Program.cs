using System;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TinyServer.ReactiveSocket;

namespace SocketClientService
{
    class Program
    {
        static Task Main(string[] args)
        {
            return CreateHostBuilder(args: args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
           .ConfigureServices((hostContext, services) =>
           {
               services.AddLogging(config =>
               {
                   config.AddConsole();
                   config.SetMinimumLevel(LogLevel.Debug);
               });
               services.AddTransient<SubscribeMessageHandle>();
               services.AddTransient<SendMessageHandle>();
               services.AddHostedService<ClientWroker>();


               /* services.AddTransient<Func<string, IShoppingCart>>(serviceProvider => key =>  
             {  
                 switch (key)  
                 {  
                     case "API":  
                         return serviceProvider.GetService<ShoppingCartAPI>();  
                     case "DB":  
                         return serviceProvider.GetService<ShoppingCartDB>();  
                     default:  
                         return serviceProvider.GetService<ShoppingCartCache>();  
                 }  
             }); */

           });

    }
}
