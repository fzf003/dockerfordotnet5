using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Streams.Dsl;
using Akka.Streams;
using Microsoft.Extensions.DependencyInjection;
namespace AkkaServerApp.Services
{
    public abstract class DomainService<TDomainService>
        where TDomainService : DomainService<TDomainService>, new()
    {
        protected IServiceProvider ServiceProvider { get; private set; }

        internal void Inject(IServiceProvider  serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }


    public class ProjectService:DomainService<ProjectService>
    {
        public Task Create()
        {
            /*var Sys = this.ServiceProvider.GetService<ActorSystem>();

          var grash=  new BusStream().Named("EventProcess");

            Source.FromGraph(grash)
                .RunForeach(p => {
                    Console.WriteLine(p);
                }, Sys.Materializer());
            */
            
     
        




            return Task.CompletedTask;
        }
    }


}
