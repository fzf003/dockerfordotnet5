using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Buffers;
using System;

namespace BedrockServer
{
    public class GreeterMiddleware
    {
        RequestDelegate _next;

        readonly LinkGenerator _linkGenerator;

        readonly ILogger logger;
        public GreeterMiddleware(RequestDelegate next, LinkGenerator linkGenerator, ILogger<GreeterMiddleware> logger)
        {
            _next = next;
            _linkGenerator = linkGenerator;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            

            var endPoint = httpContext.GetEndpoint();
            

            await _next.Invoke(httpContext);
        }
    }

    public class WriterMessage : IBufferWriter<string>
    {
        public void Advance(int count)
        {
            throw new NotImplementedException();
        }

        public Memory<string> GetMemory(int sizeHint = 0)
        {
            throw new NotImplementedException();
        }

        public Span<string> GetSpan(int sizeHint = 0)
        {
            throw new NotImplementedException();
        }
    }
}
