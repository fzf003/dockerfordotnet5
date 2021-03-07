using Microsoft.Data.SqlClient;
using NServiceBus.Transport.SqlServerNative;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Receiver
{
    public interface IQueueListener: IDisposable
    {
        Task StartAsync(CancellationToken cancellationToken = default);

        Task SendMessage(OutgoingMessage message);
    }

    public class QueueListener: IQueueListener
    {
        readonly DbConnection dbConnection;

        readonly QueueManager queueManager;

        readonly MessageLoop messageLoop;
        public QueueListener(DbConnection dbConnection,string queueName)
        {
            this.dbConnection = dbConnection;

            this.queueManager = new QueueManager(queueName, dbConnection);

            this.messageLoop = new MessageConsumingLoop(
                 table: queueName,
                 connectionBuilder: c => ConnectionBuilder(),
                 callback: async (_, message, _) =>
                 {
                     Console.WriteLine(message.Headers);
                     using var reader = new StreamReader(message.Body);
                     var result = $"{message.Id}--{message.RowVersion}--{await reader.ReadToEndAsync()}";
                     Console.WriteLine(result);
                 }, 
                 errorCallback: innerException => { Console.WriteLine(innerException.Message); }
                  , delay: TimeSpan.FromSeconds(1)
                  , batchSize: 10
                 );
        }

        public void Dispose()
        {
            this.messageLoop.Stop();
        }

        public Task SendMessage(OutgoingMessage message)
        {
            return this.queueManager.Send(message);
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            this.messageLoop.Start();
            return queueManager.Create(cancellation: cancellationToken);
        }

        static async Task<DbConnection> ConnectionBuilder()
        {
            const string connection = "server=.,14330;Initial Catalog=Receiver;User ID=sa;Password=!fzf123456;MultipleActiveResultSets=true";

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
    }
}
