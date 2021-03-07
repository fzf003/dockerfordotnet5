using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.IO;
using System.Linq;
using System;
using System.Threading.Channels;
using System.IO.Pipelines;

namespace AkkaServerApp.Services
{
    public static class TcpStream
    {
        public static void Start(ActorSystem actorSystem)
        {
            var client=  actorSystem.TcpStream().OutgoingConnection("0.0.0.0", 9900);

            var serversource = actorSystem.TcpStream().Bind("0.0.0.0", 8877);

            var souce1 = Source.From(Enumerable.Range(1, 10)).Select(p => p.ToString());
            var source2 = Source.From(Enumerable.Range(1, 10)).Select(p => $"{p}--{Guid.NewGuid().ToString()}");
            
           

            /*serversource.RunForeach(connection => { 
              connection.Flow.Join()
            }, actorSystem.Materializer());
            */

            /*flow(Sink.ForEach<ByteString>(p =>
            {

            }).RunWith(Sink.Ignore<ByteString>(), actorSystem.Materializer())
            */

 
        }
    }
}
