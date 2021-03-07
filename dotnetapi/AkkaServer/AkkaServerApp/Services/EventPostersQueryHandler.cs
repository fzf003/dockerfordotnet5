using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ServiceProvider = Akka.DependencyInjection.ServiceProvider;

namespace AkkaServerApp
{
    public class Response:IQuery<string>
    {
        public string Name { get; set; }
    }


    public class EventPostersQueryHandler : QueryHandler<Response, string>
    {

        public static Props For(ActorSystem actorSystem)
        {
            return ServiceProvider.For(actorSystem).Props<EventPostersQueryHandler>().WithRouter(new Akka.Routing.RoundRobinPool(5));
        }

        readonly ILogger logger;

        readonly IServiceProvider _serviceProvider;

        public EventPostersQueryHandler(ILogger<EventPostersQueryHandler> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task<string> ExecuteQuery(Response query)
        {
            using var servicescope=this._serviceProvider.CreateScope();
            //servicescope.ServiceProvider.GetService<>
            logger.LogInformation("Message:{0}",query.Name);
            return Task.FromResult($"{this.Self.Path.ToStringWithAddress()}--{Guid.NewGuid().ToString("N")}");
        }
    }
}
