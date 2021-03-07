using Microsoft.ReverseProxy.Abstractions;
using Microsoft.ReverseProxy.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReverseProxy
{
    public class CustomConfigFilter : IProxyConfigFilter
    {
        public ValueTask<Cluster> ConfigureClusterAsync(Cluster cluster, CancellationToken cancel)
        {
            return new ValueTask<Cluster>(cluster);
        }

        public ValueTask<ProxyRoute> ConfigureRouteAsync(ProxyRoute route, CancellationToken cancel)
        {
            // Example: do not let config based routes take priority over code based routes.
            // Lower numbers are higher priority. Code routes default to 0.
            if (route.Order.HasValue && route.Order.Value < 1)
            {
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(route));

                return new ValueTask<ProxyRoute>(route with { Order = 1, });
            }
            Console.WriteLine(route);
           // Console.WriteLine(route.Order.Value);
            return new ValueTask<ProxyRoute>(route);
        }
    }
}
