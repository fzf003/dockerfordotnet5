using Microsoft.Data.SqlClient;
using NServiceBus.Transport.SqlServerNative;
using Shard;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;

namespace Receiver
{
    class Program
    {
        const string connection = "server=.,14330;Initial Catalog=Receiver;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true";

        const string productconnection = "server=.,14330;Initial Catalog=fzf003;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true";

        static async Task<DbConnection> ConnectionBuilder()
        {
            var sqlConnection = new SqlConnection(connection);
            try
            {
                await sqlConnection.OpenAsync();
                return sqlConnection;
            }
            catch
            {
                await sqlConnection.DisposeAsync();
                throw;
            }
        }

        static OutgoingMessage BuildMessage(Guid guid)
        {
            var header = new Dictionary<string, string>()
            {
                {"top","fzf004" },
                {"channel","order" }
            };
            var json= Newtonsoft.Json.JsonConvert.SerializeObject(header);
            return new OutgoingMessage(guid,expires: DateTime.Now.AddDays(2), json, Encoding.UTF8.GetBytes($"{DateTime.Now}"));
        }

        static Task SendMessages(IQueueListener sender)
        {
            return sender.SendMessage(BuildMessage(Guid.NewGuid()));
        }

        static IQueueListener queueListener;
        static async Task Main(string[] args)
        {
            string table = "fzf-endpoint";
            var sqlConnection = await ConnectionBuilder();
            // queueListener = new QueueListener(sqlConnection, table);
            // await queueListener.StartAsync();

            var sqltablelistener= new SqlTableDependency<Product>(productconnection, "Product", includeOldValues:true);
            sqltablelistener.ActivateDatabaseLogging = true;
         

            /*  var backoff= new BackoffService();

              await backoff.Connect( context => {


                   Console.WriteLine($"{context.retry}--{DateTime.Now}--{context.errors}");
                   //
                   //if(r==7)
                   {
                       throw new Exception("aa");
                   }
               });*/





            for (; ; )
            {
         
                Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(3000))
                          .Select(p =>
                          {
                              return SendMessages(queueListener);
                          })
                          .Subscribe();

                // await SingleProduceMultipleConsumers();

                Console.WriteLine("Press any key to exit");
                Console.ReadKey();

            }

        }
 
        
        public static async Task SingleProduceMultipleConsumers()
        {
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            cancellationToken.Token.Register(() =>
            {
                Console.WriteLine("取消了....");
            });
            // cancellationToken.CancelAfter(TimeSpan.FromSeconds(10));
            var channel = Channel.CreateUnbounded<string>();

            channel.Reader.ToObservable(cancellationToken.Token)
                   .Subscribe(p => Console.WriteLine(p), ex =>
                      {
                          Console.WriteLine(ex.Message);
                      }, () =>
                      {
                          Console.WriteLine("Done.....");
                      });

            for (; ; )
            {
                var result = channel.Writer.TryWrite(Guid.NewGuid().ToString("N"));
                Console.WriteLine(result);
                Console.ReadKey();
            }

            /*var producer1 = new Producer(channel.Writer, 1, 100);
          
            var consumer2 = new Consumer(channel.Reader, 2, 1500);
            var consumer3 = new Consumer(channel.Reader, 3, 1500);

            Task consumerTask1 = consumer1.ConsumeData(); // begin consuming
            Task consumerTask2 = consumer2.ConsumeData(); // begin consuming
            Task consumerTask3 = consumer3.ConsumeData(); // begin consuming

            Task producerTask1 = producer1.BeginProducing();

            await producerTask1.ContinueWith(_ => channel.Writer.Complete());

            await Task.WhenAll(consumerTask1, consumerTask2, consumerTask3);
            */

        }
    }
}
