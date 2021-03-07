using Microsoft.Data.SqlClient;
using NServiceBus;
using NServiceBus.Logging;
using Shard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Receiver
{
    /* public class OrderSubmittedHandler :
     IHandleMessages<OrderSubmitted>
     {
         static ILog log = LogManager.GetLogger<OrderSubmittedHandler>();

         public async Task Handle(OrderSubmitted message, IMessageHandlerContext context)
         {
             log.Info($"Order {message.OrderId} worth {message.Value} submitted");

             #region StoreUserData

             var session = context.SynchronizedStorageSession.SqlPersistenceSession();

             var sql = @"insert into receiver.SubmittedOrder
                                     (Id, Value)
                         values      (@Id, @Value)";
             using (var command = new SqlCommand(
                 cmdText: sql,
                 connection: (SqlConnection)session.Connection,
                 transaction: (SqlTransaction)session.Transaction))
             {
                 var parameters = command.Parameters;
                 parameters.AddWithValue("Id", message.OrderId);
                 parameters.AddWithValue("Value", message.Value);
                 await command.ExecuteNonQueryAsync()
                     .ConfigureAwait(false);
             }

             #endregion

             #region Reply

             var orderAccepted = new OrderAccepted
             {
                 OrderId = message.OrderId,
             };
             await context.Reply(orderAccepted).ConfigureAwait(false);

             #endregion
         }

     }*/

    public class BackoffService
    {
        private static readonly Dictionary<int, TimeSpan> RetryInterval =
           new Dictionary<int, TimeSpan>()
           {
                { 6, TimeSpan.FromMilliseconds(100) },
                { 5, TimeSpan.FromMilliseconds(500) },
                { 4, TimeSpan.FromMilliseconds(1000) },
                { 3, TimeSpan.FromMilliseconds(2000) },
                { 2, TimeSpan.FromMilliseconds(4000) },
                { 1, TimeSpan.FromMilliseconds(8000) },
           };
        public async Task Connect(Action<(int retry, int errors)> action)
        {
            var retry = RetryInterval.Count + 1;
            var exceptions = new List<Exception>();
            while (true)
            {
                try
                {
                    action((retry, exceptions.Count));
                    return;
                }
                catch (Exception e)
                {
                    retry--;
                    if (retry == 0)
                        throw new AggregateException("Failed to connect to AMQP.V1 server.", exceptions);

                    exceptions.Add(e);
                    await Task.Delay(RetryInterval[retry]);

                }
            }
        }
    }
}
