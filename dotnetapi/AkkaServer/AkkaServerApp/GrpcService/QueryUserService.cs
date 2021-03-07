using Akka.Client.Autogen.Grpc.v1;
using AkkaServer.Core;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static Akka.Client.Autogen.Grpc.v1.UserService;
using Akka.Actor;
namespace AkkaServerApp.GrpcService
{
    public class QueryUserService:UserServiceBase
    {
        readonly ActorRefProvider<ResponseActor> _actorRefProvider;
        readonly ILogger _logger;
        readonly HttpClient httpClient;
        public QueryUserService(ActorRefProvider<ResponseActor> actorRefProvider, ILogger<QueryUserService> logger, IHttpClientFactory httpClient)
        {
            _actorRefProvider = actorRefProvider;
            _logger = logger;
            this.httpClient = httpClient.CreateClient("http");
        }

        public override async Task QueryStreamUsers(RequestMessae request, IServerStreamWriter<ReplyMessage> responseStream, ServerCallContext context)
        {
            while(!context.CancellationToken.IsCancellationRequested)
            {
                await responseStream.WriteAsync(new ReplyMessage
                {
                    Message = DateTime.Now.ToString()
                });
            }
        }

        public override  Task<ReplyMessage> QueryUsers(RequestMessae request, ServerCallContext context)
        {
               this.httpClient.GetStringAsync("all.json")
                         .PipeTo(_actorRefProvider.ActorRef);

            _logger.LogInformation($"收到:{request.Message}");
            return 
                _actorRefProvider.Ask<ReplyMessage>(request);
        }
         
    }
}
