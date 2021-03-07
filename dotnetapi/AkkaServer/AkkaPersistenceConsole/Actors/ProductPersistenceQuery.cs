using Akka.Actor;
using Akka.Persistence.MongoDb.Query;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPersistenceConsole.Actors
{
    public class ProductPersistenceQuery : ReceiveActor
    {
        public class Start
        {
            public static Start Instance = new Start();
            private Start() { }

        }
        public static Props PropsFor(string bookid)
        {
            return Props.Create(() => new ProductPersistenceQuery(bookid));
        }

        protected override void PreStart()
        {
            this.Self.Tell(Start.Instance);
            base.PreStart();
        }

        public ProductPersistenceQuery(string bookid)
        {
            var actorSystem = Context.System;

            this.Receive<Start>(p => {
                //Akka.Persistence.MongoDb.Query.MongoDbReadJournal
                var reader = actorSystem.ReadJournalFor<MongoDbReadJournal>(MongoDbReadJournal.Identifier);
                //ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);
 
                reader.EventsByPersistenceId($"{bookid}", 0, long.MaxValue)
                      .RunForeach(p =>
                      {
                          Console.WriteLine($"读取当前Actor:book-{bookid} 事件:{JsonConvert.SerializeObject(p.Event)}");
                      }, actorSystem.Materializer());
            });
        }
    }
}
