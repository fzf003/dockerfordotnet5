using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyServer.ReactiveSocket;
using System.Reactive.Threading;
using System.Reactive.Threading.Tasks;
using System.Reactive.Disposables;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace PipeChannelService
{
    class Program
    {
        static Task Main(string[] args)
        {
            return CreateHostBuilder(args).Build().RunAsync();
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
                     services.AddHostedService<ServerWorker>();
                 });



        static void StartServer(ILoggerFactory loggerfactory, ILogger<Program> logger)
        {
            var server = SocketServer.CreateServer(new IPEndPoint(IPAddress.Any, 8086), loggerfactory);

            server.AcceptClientObservable.Subscribe(client =>
            {
                client.RevicedObservable.Subscribe(async bytes =>
               {
                   var message = Encoding.UTF8.GetString(bytes);

                   logger.LogInformation(message);

                   await client.SendMessageAsync(DateTime.Now.ToString());

               }, ex =>
                  {
                      logger.LogError($"{ex.Message}");

                  });

            });

            server.Start();
        }
    }





    public class ObservableConsole : IObservable<string>
    {
        public IDisposable Subscribe(IObserver<string> observer)
        {
            return null;
        }
    }


}
