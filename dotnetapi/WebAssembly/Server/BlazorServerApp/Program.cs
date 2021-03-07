using BlazorServerApp.Middleware;
using BlazorServerApp.Service;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BlazorServerApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddLogging(cfg =>
            {
                cfg.SetMinimumLevel(LogLevel.Debug);

            });
            builder.Services.AddTransient<HttpClientDiagnosticsHandler>();
           
            builder.Services.AddHttpClient<HttpService>(cfg =>
            {
                cfg.BaseAddress = new Uri("http://v1.jinrishici.com");
            }).AddHttpMessageHandler<HttpClientDiagnosticsHandler>();

             builder.Services.AddScoped(sp => new HttpClient(sp.GetService<HttpClientDiagnosticsHandler>()) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
