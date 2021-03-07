using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BedrockServer
{
    public class TestHost : IHostedService
    {
        readonly ILogger logger;

        readonly IServer server;

        public TestHost(ILogger<TestHost> logger, IServer server)
        {
            this.logger = logger;
            this.server = server;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("开始...");

            var features = this.server.Features.Get<IServerAddressesFeature>();
           if( features.Addresses.Any())
            {
                foreach(var item in features.Addresses)
                {
                    this.logger.LogInformation(item);
                }
            }
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("停止...");
            return Task.CompletedTask;
        }
    }
}
