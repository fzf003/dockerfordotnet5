using Microsoft.Extensions.Logging;
using PipeChannelService.ReactiveSocket;
using System;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace PipeChannelService
{
    class Program
    {
        static void PrintLog(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(message);

            Console.ResetColor();
        }

        static void PrintError(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(exception.Message);

            Console.ResetColor();
        }
        
        static Socket StartClinet(int port = 9001)
        {
            var clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine($"Connecting to port {port}");

            clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, port));

            return clientSocket;
        }
 
        static async Task Main(string[] args)
        {
           var loggerfactory= LoggerFactory.Create(options =>
            {
                options.SetMinimumLevel(LogLevel.Information)
                       .AddConsole();
                
            });

            var logger = loggerfactory.CreateLogger<Program>();


            CancellationTokenSource cancellationToken = new CancellationTokenSource();

           using var server = new SocketServer(new IPEndPoint(IPAddress.Loopback, 8087),loggerfactory);
            
            server.AcceptClientObservable.Subscribe(client =>
            {
                client.RevicedObservable.Subscribe( bytes =>
                {
                    var message=Encoding.UTF8.GetString(bytes);

                    logger.LogInformation(message);

                   // await client.Writer.SendAsync(Encoding.UTF8.GetBytes($"{message}\n"));

                }, ex =>
                {
                    logger.LogError($"{ex.Message}");

                });

            });

            server.Start();


            Console.WriteLine("...................");
            

            var client = StartClinet(8087);

           // var netreader = PipeReader.Create(new NetworkStream(client));

            var netwriter = PipeWriter.Create(new NetworkStream(client));

          /*  SocketClient.ReaderAsync(netreader)
                        .Do(PrintLog, PrintError)
                        .Subscribe();*/

            for (; ; )
            {

                File.ReadAllLines("1.txt").ToObservable()
                                          .Select(message => Observable.FromAsync(() => SocketClient.OneWriteHelloAsync(writer: netwriter, message)))
                                          .Concat()
                                          .Subscribe(p => { }, PrintError, async () =>
                                            {
                                                //await netwriter.CompleteAsync();
                                                Console.WriteLine("Done......");
                                            });

                Console.WriteLine(".....................");

                Console.ReadKey();
            }
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
