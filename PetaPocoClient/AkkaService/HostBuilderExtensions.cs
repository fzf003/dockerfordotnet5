using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetaPocoClient.AkkaService
{
    public static class HostBuilderExtensions
    {
        
        public static IHostBuilder UseAkkaServer(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((context, services) => {
              
            });
        }
    }
}
