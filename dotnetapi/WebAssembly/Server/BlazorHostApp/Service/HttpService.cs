using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorHostApp.Service
{
    public class HttpService
    {
        readonly HttpClient httpClient;

        readonly ILogger<HttpService> logger;

        public HttpService(HttpClient httpClient, ILogger<HttpService> logger)
        {
            this.httpClient = httpClient;

            this.logger = logger;
        }

        public Task<string> GetMessages()
        {
            return 
                //Task.FromResult(Guid.NewGuid().ToString()); 
                this.httpClient.GetStringAsync("all.json");
        }
    }
}
