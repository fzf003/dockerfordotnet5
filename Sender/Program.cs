using Microsoft.Data.SqlClient;
using NServiceBus;
using Shard;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;

namespace Sender
{
    class Program
    {
        const string sqlconnection = "server=.,14330;Initial Catalog=fzf003;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true";
        const string sqlselect = "select  * from ProductNew where Status=130 ";
        static void Main(string[] args)
        {

            CancellationTokenSource cancellationToken = new CancellationTokenSource();
           // cancellationToken.CancelAfter(TimeSpan.FromSeconds(5));


            var channel = Channel.CreateUnbounded<Product>();

            var datastream = DatabaseObservable.CreateQuerySqlCommand<Product>(sqlconnection, sqlselect, CancellationToken.None)
                                               .Do(p => {
                                                     Console.WriteLine(p);
                                                 }, err => { Console.WriteLine(err.Message); })            
                                               .Retry();
 
            var timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(10));

            var writer=new DatabaseMessageChannelWriter(dbConnection());

            var publisher = Observable.Range(1, 100);
            publisher.Do(async p =>
            {
                await Task.Delay(1000);
            });
            // .Subscribe(ObservableProduct(cancellationToken.Token));
           //datastream.WriterToDatabase(writer);
           datastream.Subscribe(Console.WriteLine);


            Console.WriteLine("......");
            Console.ReadKey();
            cancellationToken.Cancel();
            Console.ReadKey();
          

            
        }

        static IObserver<int> ObservableProduct(CancellationToken cancellation)
        {
           var linkoptions= new DataflowLinkOptions
            {
                Append = true,
                PropagateCompletion = true
                
            };

            var blockoptions = new ExecutionDataflowBlockOptions
            {
                EnsureOrdered = true,
                MaxDegreeOfParallelism = 1,
                SingleProducerConstrained=true,
                CancellationToken= cancellation,
                BoundedCapacity=1
            };

            var  actionBlock = new ActionBlock<RequestMessage>(p =>
            {
                Console.WriteLine($"Action:{p.Num}--Thread:{Thread.CurrentThread.ManagedThreadId}");

            });

            var failBlock = new ActionBlock<RequestMessage>(p => {
                Console.WriteLine($"Fail:{p.Num}-Thread:{Thread.CurrentThread.ManagedThreadId}");
            });

            var buffer = new BufferBlock<int>();
            var tranfrom = new TransformBlock<int, RequestMessage>(async p => {
                if (p % 2 == 0)
                {
                    return new RequestMessage(false, p);
                }
                return new RequestMessage(true, p);
            });

            buffer.LinkTo(tranfrom, linkoptions);
            tranfrom.LinkTo(actionBlock, linkoptions,p=>p.Status==true);
            tranfrom.LinkTo(failBlock, linkoptions,p=>p.Status==false);

            actionBlock.Completion.ContinueWith(p => {
                Console.WriteLine("action Task Done......");
            });

            return buffer.AsObserver();
        }

        static DbConnection dbConnection()
        {
            var factory = SqlClientFactory.Instance;
            var conn = factory.CreateConnection();
            conn.ConnectionString = sqlconnection;
            conn.Open();
            return conn;
        }

        static async Task Start()
        {
            var endpointConfiguration = new EndpointConfiguration("SqlOutbox.Sender");
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.SendFailedMessagesTo("error");

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(sqlconnection);
            transport.DefaultSchema("sender");
            transport.UseSchemaForQueue("error", "dbo");
            transport.UseSchemaForQueue("audit", "dbo");
            transport.NativeDelayedDelivery().EnableTimeoutManagerCompatibility();

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.ConnectionBuilder(
                connectionBuilder: () =>
                {
                    return new SqlConnection(sqlconnection);
                });

            var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();
            dialect.Schema("sender");
            persistence.TablePrefix("");

            var subscriptions = transport.SubscriptionSettings();
            subscriptions.DisableSubscriptionCache();

            subscriptions.SubscriptionTableName(
                tableName: "Subscriptions",
                schemaName: "dbo");

            endpointConfiguration.EnableOutbox();

            SqlHelper.CreateSchema(sqlconnection, "sender");
            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            Random random = new Random();
            for (; ; )
            {
                var orderSubmitted = new OrderSubmitted
                {
                    OrderId = Guid.NewGuid(),
                    Value = random.Next(100)
                };
                await endpointInstance.Publish(orderSubmitted)
                    .ConfigureAwait(false);
                Console.WriteLine("......");
                Console.ReadKey();
            }
        }
    }


    public class RequestMessage
    {
        public RequestMessage(bool status, int num)
        {
            Status = status;
            Num = num;
        }

        public bool Status { get; }

        public int Num { get; }
    }
}
