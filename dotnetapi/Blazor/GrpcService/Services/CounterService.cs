using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcService.Services
{
    public class CounterService:Counter.CounterBase
    {
        private readonly ILogger<CounterService> _logger;
        public CounterService(ILogger<CounterService> logger)
        {
            _logger = logger;
        }

        public override async Task StartCounter(CounterRequest request, IServerStreamWriter<CounterResponse> responseStream, ServerCallContext context)
        {
            var count = request.Start;

            while (!context.CancellationToken.IsCancellationRequested)
            {

                await responseStream.WriteAsync(new CounterResponse()
                {
                    End = count++
                });
               await Task.Delay(10);
            }



            //return base.StartCounter(request, responseStream, context);
        }

        [Authorize]
        public override   Task<CounterResponse> QueryByUserId(CounterRequest request, ServerCallContext context)
        {

            var user = context.GetHttpContext().User;

            return Task.FromResult(new CounterResponse
            {
                End = 900,
                 User=user.Identity.Name
            });


            
        }
    }
}
