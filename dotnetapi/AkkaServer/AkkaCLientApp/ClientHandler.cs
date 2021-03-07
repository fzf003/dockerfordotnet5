using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AkkaCLientApp
{
    public class ClientHandler : DelegatingHandler
    {
        readonly ILoggerFactory loggerFactory;
        readonly ILogger logger;
        public ClientHandler()
        {
            loggerFactory= LoggerFactory.Create(builder => builder.AddConsole());
            logger=loggerFactory.CreateLogger<ClientHandler>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var current = request.RequestUri;
            try
            {
                logger.LogInformation($"请求开始:{request.RequestUri.Host}--{request.Method}--{request.RequestUri}");

                var response= await this.SendAsync(request, cancellationToken);

                logger.LogInformation($"请求结束:{request.RequestUri.Host}--{request.Method}--{request.RequestUri}");

                return response;
                
            }catch(Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
            finally
            {
                request.RequestUri = current;
            }
        }
    }
}
