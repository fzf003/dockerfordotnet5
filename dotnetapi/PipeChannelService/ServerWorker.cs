namespace PipeChannelService
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Reactive.Disposables;
    using TinyServer.ReactiveSocket;
    using System.Net;
    using System.Text;

    public class ServerWorker : IHostedService
    {
        readonly ILogger<ServerWorker> _logger;

        IDisposable disposable = Disposable.Empty;

        ISocketServer _socketServer;

        readonly ILoggerFactory _logFactory;
        public ServerWorker(ILogger<ServerWorker> logger, ILoggerFactory logFactory)
        {
            _logger = logger;

            _logFactory = logFactory;

            _socketServer = SocketServer.CreateServer(new IPEndPoint(IPAddress.Any, 8086), _logFactory);

            disposable = _socketServer.AcceptClientObservable.Subscribe(AccceptClient, PrintError, OnServerCompole);

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _socketServer.Start();

 
            return Task.CompletedTask;
        }

        void AccceptClient(ISocketAcceptClient acceptClient)
        {
           acceptClient.RevicedObservable
                       .Select(bytes => bytes.ToMessage())
                       .Subscribe(p=>PrintMessage(p,acceptClient), PrintError,OnClientCompole);
        } 

        void PrintMessage(string message,ISocketAcceptClient client)
        {
            _logger.LogInformation(message);
            
            client.SendMessageAsync($"Server Reply:{DateTime.Now.ToString()}".ToMessageBuffer());
        }

        void OnClientCompole()
        {
            _logger.LogInformation("client Done......");
        }

         void OnServerCompole()
        {
            _logger.LogInformation("Server Done......");
        }

        void PrintError(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            _logger.LogError(exception.Message);

            Console.ResetColor();
        }



        public Task StopAsync(CancellationToken cancellationToken)
        {
            _socketServer.Dispose();
            disposable.Dispose();
            _logger.LogInformation("停止......");
            return Task.CompletedTask;
        }


    }
}