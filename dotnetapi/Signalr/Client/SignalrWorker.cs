using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Client
{
    public class SignalrWorker : IHostedService
    {

        readonly ILogger<SignalrWorker> _logger;

        readonly HubConnection _connection;

        public SignalrWorker(ILogger<SignalrWorker> logger, HubConnection connection)
        {
            _logger = logger;

            _connection = connection;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _connection.Closed += (ex) =>
            {
                _logger.LogError(ex.Message);

                return Task.CompletedTask;
            };

            _connection.On<string>("AddUser", p =>
             {
                 _logger.LogInformation($"用户动态:{p}");
             });

            _connection.StartAsync(cancellationToken);

            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
                     .Select(p => Observable.FromAsync(() => _connection.InvokeAsync("OnAddUser", p.ToString())))
                     .Concat()
                     .Retry()
                     .Catch<System.Reactive.Unit, Exception>(ex =>
                     {
                         _logger.LogError("Error:{0}", ex.Message);
                         return Observable.Empty<System.Reactive.Unit>();
                     })
                     .Subscribe();


            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}