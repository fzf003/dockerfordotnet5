
using System;
using System.Threading.Tasks;
using EchoService.Services;
using Echotell;
using Greet;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace EchoService
{
    public class UserInfoService: Teller.TellerBase
    {
        private readonly ILogger<UserInfoService> _logger;
 

        readonly IChangeStream changeStream;

        public UserInfoService(IChangeStream changeStream, ILogger<UserInfoService> logger)
        {
            _logger = logger;

            this.changeStream = changeStream;
        }

        public override Task TellResponse(UserRequest request, IServerStreamWriter<UserResponse> responseStream, ServerCallContext context)
        {

            this.changeStream.OnChange +=  async (response) =>
            {
                _logger.LogInformation(response.Body);

               await responseStream.WriteAsync(response);

                //await Task.Delay(5);
            };

            this.changeStream.OnProcess(request,context.CancellationToken);


            return Task.CompletedTask;
        }

        
    }
}