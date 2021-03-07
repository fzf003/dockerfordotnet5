using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorServerApp.Middleware
{
    public class HttpClientDiagnosticsHandler : DelegatingHandler
    {
        readonly ILogger<HttpClientDiagnosticsHandler> logger;
        /* public HttpClientDiagnosticsHandler(HttpMessageHandler innerHandler, ILogger<HttpClientDiagnosticsHandler> logger) : base(innerHandler)
         {
             this.logger = logger;
         }*/

        public HttpClientDiagnosticsHandler(ILogger<HttpClientDiagnosticsHandler> logger)
        {
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var totalElapsedTime = Stopwatch.StartNew();

            logger.LogDebug(string.Format("Request: {0}", request));
            if (request.Content != null)
            {
                var content = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogDebug(string.Format("Request Content: {0}", content));
            }

            var responseElapsedTime = Stopwatch.StartNew();
            var response = await base.SendAsync(request, cancellationToken);

            logger.LogDebug(string.Format("Response: {0}", response));
            if (response.Content != null)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogDebug(string.Format("Response Content: {0}", content));
            }

            responseElapsedTime.Stop();
            logger.LogDebug(string.Format("Response elapsed time: {0} ms", responseElapsedTime.ElapsedMilliseconds));

            totalElapsedTime.Stop();
            logger.LogDebug(string.Format("Total elapsed time: {0} ms", totalElapsedTime.ElapsedMilliseconds));

            return response;
        }
    }
}
