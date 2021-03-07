using Akka.Actor;
using Akka.Persistence.MongoDb;
using AkkaPersistenceConsole.Actors;
using AkkaServer.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AkkaPersistenceConsole
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;

        readonly ActorSystem _actorSystem;
 
        public Worker(ILogger<Worker> logger, ActorSystem actorSystem)
        {
            _logger = logger;

         
            
            _actorSystem = actorSystem;
        }
        public  Task StartAsync(CancellationToken cancellationToken)
        {
            var productactor= this._actorSystem.ActorOf(ProductMasterActor.PropsFor(),"productmaster");
            var query = this._actorSystem.ActorOf(ProductPersistenceQuery.PropsFor("1"),"query");
            int i = 780;
            productactor.Forward(Print.Instance);
            
            _actorSystem.Scheduler.Advanced.ScheduleRepeatedly(TimeSpan.Zero, TimeSpan.FromSeconds(5), () =>
            {
                /* productactor.Forward(new CreateProductDraftCommand
                 {
                     ProductId = 1,
                     ProductName = "秋风引$$"+(i++),
                     Category = "古诗文-四季-秋天",
                     Summary = "秋风引",
                     Description = "何处秋风至？萧萧送雁群。",
                     ImageFile = "www.baidu.com",
                     Price = 99.8m,
                     Status = Models.ProductStatus.Draft
                 });*/
                
               // productactor.Forward(new ChangeNameCommand(1,$"我的秋风引:{(i++)}"));

                productactor.Forward(Print.Instance);


            });
            

            _logger.LogInformation("Start....");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop....");
            return Task.CompletedTask;
        }

         
    }

}
