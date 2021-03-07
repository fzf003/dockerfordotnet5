using Microsoft.Data.SqlClient;
using NServiceBus;
using NServiceBus.Logging;
using Shard;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
namespace Sender
{
    public class OrderAcceptedHandler : IHandleMessages<OrderAccepted>
    {
        static ILog log = LogManager.GetLogger<OrderAcceptedHandler>();

        public Task Handle(OrderAccepted message, IMessageHandlerContext context)
        {
            log.Info($"Order {message.OrderId} accepted.");
            return Task.CompletedTask;
        }
    }

    public class DatabaseProcess : IObserver<IList<Product>>
    {
        readonly DatabaseMessageChannelWriter channelWriter;

        public DatabaseProcess(DatabaseMessageChannelWriter channelWriter)
        {
            this.channelWriter = channelWriter;
        }

        public void OnCompleted()
        {
            channelWriter.Complete();
        }

        public void OnError(Exception error)
        {
            channelWriter.Complete(error);
            Console.WriteLine(error.Message);
        }

        public void OnNext(IList<Product> messages)
        {
            channelWriter.WriteAsync(messages).AsTask()
                .ContinueWith(t => {
                  if(t.IsCompletedSuccessfully)
                    {
                        Console.WriteLine("成功");
                    }
                });
           // Console.WriteLine(result);
        }
    }
}
