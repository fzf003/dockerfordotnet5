using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Event;
using Akka.Routing;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using ServiceProvider = Akka.DependencyInjection.ServiceProvider;
using Akka.Client.Autogen.Grpc.v1;
using System.Threading.Tasks;

namespace AkkaServerApp
{
    public class ResponseActor:ReceiveActor
    {
        public static Props For(IServiceProvider  serviceProvider)
        {
            return ServiceProvider.For(serviceProvider.GetService<ActorSystem>()).Props<ResponseActor>().WithRouter(FromConfig.Instance);
        }
 
        readonly ILogger _logger;
        public ResponseActor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ResponseActor>();
           // Context.System.EventStream.Subscribe(Self, typeof(DeadLetter));
            this.Receive<RequestMessae>(Handle);
            Receive<DeadLetter>(Handle);
            Receive<string>(p => {
                _logger.LogInformation(p);
            });
        }

        void Handle(RequestMessae message)
        {
            Sender.Tell(new ReplyMessage
            {
                Message = $"Message:{message.Message}--{this.Self.Path.ToString()}"
            });
             
            _logger.LogInformation($"Message:{message.Message}--{this.Self.Path.ToString()}");
        }

        void Handle(DeadLetter deadLetter)
        {
            _logger.LogInformation(deadLetter.Message+"--"+deadLetter.Recipient.Path.ToStringWithAddress());
        }
 
    }

   

}