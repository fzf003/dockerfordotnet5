using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GrpcService.Counter;
using Grpc.Core;
using System.Threading;

namespace WebAssemblyClient.Pages
{
    public partial class Counter
    {
        private int currentCount = 0;

        private CancellationTokenSource? cts;

         async Task IncrementCount()
        {
            
            cts = new CancellationTokenSource();

            var client = new CounterClient(Channel);
             var call = client.StartCounter(new GrpcService.CounterRequest
            {
                Start = currentCount
            },cancellationToken: cts.Token);


            try
            {
                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    currentCount = message.End;
                    StateHasChanged();
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                // Ignore exception from cancellation
            }
        }

         void StopCount()
        {
            cts?.Cancel();
            cts = null;
        }
    }
}
