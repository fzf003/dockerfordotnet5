using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProductApi
{
    public class HostService : IHostedService
    {
        readonly ILogger<HostService> _logger;

        private readonly IServer _server;

        readonly IServerAddressesFeature _addressesFeature;

        public HostService(ILogger<HostService> logger, IServer server, IServerAddressesFeature addressesFeature)
        {
            _logger = logger;
            _server = server;

            _addressesFeature = addressesFeature;
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("开始....");
            //var features = _addressesFeature;


            var address =_server.Features.Get<IServerAddressesFeature>();

            _logger.LogInformation("addcount:{0}--{1}",address.Addresses.Count, address.PreferHostingUrls);
            foreach (var item in address.Addresses)
            {
                _logger.LogInformation($"address:{item}");
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("停止....");
            return Task.CompletedTask;
        }
    }
}
