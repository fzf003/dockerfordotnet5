using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AkkaServer.Core
{
    public class AkkaServerHost : IHostedService
    {
        readonly AkkaOptions _akkaOptions;
        readonly IServiceProvider _serviceProvider;

        public ActorSystem ActorSystem { get; set; }
        public AkkaServerHost(IOptions<AkkaOptions> options, IServiceProvider serviceProvider)
        {
            this._akkaOptions = options.Value;
             
            this._serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = HoconLoader.ParseConfig(this._akkaOptions.Configfile);
            if (this._akkaOptions.IsDocker)
                config = config.BootstrapFromDocker();
            var bootstrap = BootstrapSetup.Create().WithConfig(config);
            var di = ServiceProviderSetup.Create(this._serviceProvider);
            var actorSystemSetup = bootstrap.And(di);
             
            
           this.ActorSystem= ActorSystem.Create(config.GetString(this._akkaOptions.Name), actorSystemSetup).StartPbm();

            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this.ActorSystem.Terminate();
        }
    }
}
