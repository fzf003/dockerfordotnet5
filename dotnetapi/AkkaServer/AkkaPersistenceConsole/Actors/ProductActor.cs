using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Event;
using Akka.Persistence;
using AkkaPersistenceConsole.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPersistenceConsole.Actors
{
    public class ProductActor : ReceivePersistentActor
    {
        ILoggingAdapter logger = Context.GetLogger();
        public override string PersistenceId { get; }
        public const int SnapshotInterval = 10;
        Product productState;
        public ProductActor(string productid)
        {

            this.PersistenceId = productid;

            Commands();

            Recover();
        }

        public static Props PropsFor(string productid)
        {
            return Props.Create<ProductActor>(productid);
        }
        

        private void Commands()
        {
            this.Command<ChangeNameCommand>(ChangeCommand);

            this.Command<CreateProductDraftCommand>(ProcessCommand);

            this.Command<Print>(p => {
                this.logger.Info("当前状态:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(this.productState));
            });

            Command<SaveSnapshotSuccess>(s =>
            {
                Console.WriteLine("保存快照成功");
                DeleteSnapshots(new SnapshotSelectionCriteria(s.Metadata.SequenceNr - 1));
                DeleteMessages(s.Metadata.SequenceNr);
            });
        }

        private void Recover()
        {
            this.Recover<CreateProductEvent>(e => {
                this.logger.Info("事件恢复:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(e));
                ConvertToProduct(e);
            });

            this.Recover<ChangeNameEvent>(@event => {
                this.productState.ChangeName(@event.ReName);
                Console.WriteLine("恢复ChangeNameEvent：{0}", @event.ReName);
            });
            //Akka.Persistence.
            Recover<SnapshotOffer>(offer =>
            {
                this.logger.Info("快照恢复:{0}",Newtonsoft.Json.JsonConvert.SerializeObject( offer.Snapshot));
                if (offer.Snapshot is Product product)
                {
                    this.productState = product;
                }
            });

            
        }

        void ChangeCommand(ChangeNameCommand changeNameCommand)
        {
            var @event = new ChangeNameEvent(changeNameCommand.Id, changeNameCommand.ReName);

            this.PersistAsync(@event, changeevent =>
            {
                this.productState.ChangeName(@event.ReName);

                if (LastSequenceNr % SnapshotInterval == 0)
                {
                    Console.WriteLine("快照:{0}", LastSequenceNr);
                    this.SaveSnapshot(this.productState);
                }

            });
        }

        private void ProcessCommand(CreateProductDraftCommand productDraftCommand)
        {
            var @event = new CreateProductEvent()
            {
                ProductId = productDraftCommand.ProductId,
                Category = productDraftCommand.Category,
                Description = productDraftCommand.Description,
                ImageFile = productDraftCommand.ImageFile,
                Price = productDraftCommand.Price,
                ProductName = productDraftCommand.ProductName,
                Status = productDraftCommand.Status,
                Summary = productDraftCommand.Summary
            };

            this.PersistAsync(@event, e => {

                Sender?.Tell(e);
                ConvertToProduct(e);
                logger.Info("发布事件:{0}", JsonConvert.SerializeObject(@event));
                Context.System.EventStream.Publish(e);

                if (LastSequenceNr % SnapshotInterval == 0)
                {
                    Console.WriteLine("快照:{0}", LastSequenceNr);
                    this.SaveSnapshot(this.productState);
                }
                

            });
        }

        void ConvertToProduct(CreateProductEvent productevent)
        {
            this.productState = Product.Create(name: productevent.ProductName, category: productevent.Category, description: productevent.Description, image: productevent.ImageFile, price: productevent.Price);
        }
    }
}
