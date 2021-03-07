using Akka.Actor;
using AkkaServer.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace AkkaServerApp
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;

        readonly ActorSystem _actorSystem;
        readonly ActorRefProvider<ResponseActor> _actorRefProvider;
 
        public Worker(ILogger<Worker> logger, ActorRefProvider<ResponseActor> actorRefProvider,   ActorSystem actorSystem)
        {
            _logger = logger;

            _actorRefProvider = actorRefProvider;
            
            _actorSystem = actorSystem;
        }
        public  Task StartAsync(CancellationToken cancellationToken)
        {
             _logger.LogInformation("Start....");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop....");
            return Task.CompletedTask;
        }

         
    }

}
