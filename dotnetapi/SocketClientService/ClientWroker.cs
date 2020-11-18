namespace SocketClientService
{
    using System.Net;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using TinyServer.ReactiveSocket;
    using System.Reactive.Disposables;
    using System;
    using System.Reactive;




    public class ClientWroker : IHostedService
    {
        readonly ILoggerFactory _logfactory;

        ISocketClient _socketClient;

        readonly CompositeDisposable _compositedisp;

        readonly ILogger<ClientWroker> _logger;

        readonly SendMessageHandle sendMessageHandle;
        readonly SubscribeMessageHandle subMessageHandle;
        public ClientWroker(ILoggerFactory logfactory, SendMessageHandle sendMessageHandle, SubscribeMessageHandle subMessageHandle)
        {
            _logfactory = logfactory;

            _logger = logfactory.CreateLogger<ClientWroker>();

            _compositedisp = new CompositeDisposable();

            this.sendMessageHandle = sendMessageHandle;

            this.subMessageHandle = subMessageHandle;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {


            Starting();

            return Task.CompletedTask;
        }
        void Starting()
        {
            _logger.LogInformation("Starting................");
            var ip =IPAddress.Parse(System.Environment.GetEnvironmentVariable("IP"));

            _socketClient = SocketClient.CreateClient(new IPEndPoint(ip, 9300), _logfactory);
            var disp = _socketClient.ReceiveMessageObservable
                                    .Select(bytes => bytes.ToMessage())
                                    .Subscribe(this.subMessageHandle);

            var senddisp = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                                     .Select(_ => Observable.FromAsync(() => _socketClient.SendMessageAsync($"{Guid.NewGuid().ToString()}")))
                                     .Concat()
                                     .Subscribe(this.sendMessageHandle);



            _compositedisp.Add(disp);

            _compositedisp.Add(senddisp);

            _logger.LogInformation("Started................");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ShutDown();
            _logger.LogInformation("ClientWorker Stop!!!!!");
            return Task.CompletedTask;
        }

        void ShutDown()
        {
            _logger.LogInformation("客户端关闭.....");
            _compositedisp?.Dispose();
            _socketClient?.Dispose();
        }
    }
}