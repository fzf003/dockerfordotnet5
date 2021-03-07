using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReverseProxy
{
    public static class HostBuilderExtensions
    {
        public static IWebHostBuilder BindToPorts(this IWebHostBuilder webHostBuilder, int? runLocalHttpPort, int? runLocalHttpsPort)
        {
            var urls = new List<string>();

            var portStr = Environment.GetEnvironmentVariable("PORT") ?? Environment.GetEnvironmentVariable("SERVER_PORT");
            var aspnetUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
            if (!string.IsNullOrWhiteSpace(portStr))
            {
                if (int.TryParse(portStr, out var port))
                {
                    urls.Add($"http://*:{port}");
                }
                else if (portStr.Contains(";"))
                {
                    if (!string.IsNullOrEmpty(aspnetUrls))
                    {
                        urls.AddRange(aspnetUrls.Split(';'));
                    }
                    else
                    {
                        var ports = portStr.Split(';');
                        urls.Add($"http://*:{ports[0]}");
                        urls.Add($"https://*:{ports[1]}");
                    }
                }
            }
            
            else
            {
                if (runLocalHttpPort != null)
                {
                    urls.Add($"http://*:{runLocalHttpPort}");
                }

                if (runLocalHttpsPort != null)
                {
                    urls.Add($"https://*:{runLocalHttpsPort}");
                }
            }

            if (urls.Count > 0)
            {
                webHostBuilder.UseUrls(urls.ToArray());
            }
            else
            {
                webHostBuilder.UseUrls(new string[] { "http://*:8080" });
            }

            return webHostBuilder;
        }
    }
}

