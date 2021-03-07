using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorServerApp.Service
{
    public class HttpService
    {
        readonly HttpClient httpClient;

       

        public HttpService(HttpClient httpClient) 
        {
            this.httpClient = httpClient;
        }

        public Task<string> GetMessages()
        {
           // return Task.FromResult(Guid.NewGuid().ToString("N"));
            return this.httpClient.GetStringAsync("all.json");
        }
    }
}
