using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPersistenceConsole.Actors
{
    public sealed class ProductMasterActor : ReceiveActor
    {
        public static Props PropsFor()
        {
            return Props.Create<ProductMasterActor>();
        }
        public ProductMasterActor()
        {
            Receive<CreateProductDraftCommand>(s =>
            {
                var orderbookActor = Context.Child(s.ProductId.ToString()).GetOrElse(() => StartChild(s.ProductId.ToString()));
                orderbookActor.Forward(s);
            });

            Receive<ChangeNameCommand>(s =>
            {
                var orderbookActor = Context.Child(s.Id.ToString()).GetOrElse(() => StartChild(s.Id.ToString()));
                orderbookActor.Forward(s);
            });

            Receive<Print>(p => {
                var productActor = Context.Child("1").GetOrElse(() => StartChild("1"));
                productActor.Forward(p);
            });
        }

        private IActorRef StartChild(string productid)
        {
            return Context.ActorOf(ProductActor.PropsFor(productid), productid);
        }
    }
}
