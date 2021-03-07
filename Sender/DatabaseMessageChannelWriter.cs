using Dapper;
using Shard;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sender
{
    public class DatabaseMessageChannelWriter : AbstractMessageChannelWriter<IList<Product>>
    {
        readonly DbConnection dbConnection;

        const string updatesql = "update [ProductNew]  set Status=170 where Id in @ids ";

        const string insertsql = @"INSERT INTO [t1]
                                   ([Name]
                                   ,[Category]
                                   ,[Summary]
                                   ,[Description]
                                   ,[ImageFile]
                                   ,[Price]
                                   ,[Status]
                                    ,[CreateTime])
                             VALUES
                                   (@Name,
                                    @Category,
                                    @Summary,
                                    @Description,
                                   @ImageFile,
                                   @Price, 
                                   @Status,
                                   @CreateTime)";

        const string inserttwosql = @"INSERT INTO [t2]
                                   ([Name]
                                   ,[Category]
                                   ,[Summary]
                                   ,[Description]
                                   ,[ImageFile]
                                   ,[Price]
                                   ,[Status]
                                    ,[CreateTime])
                             VALUES
                                   (@Name,
                                    @Category,
                                    @Summary,
                                    @Description,
                                   @ImageFile,
                                   @Price, 
                                   @Status,
                                   @CreateTime)";


        public DatabaseMessageChannelWriter(DbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }
        public override ValueTask WriteAsync(IList<Product> messages, CancellationToken cancellationToken = default)
        {
             return new ValueTask(WriterDatabase(messages));
        }

        async Task WriterDatabase(IList<Product> messages)
        {
            using var tran = this.dbConnection.BeginTransaction();
            
                try
                {
                    Console.WriteLine($"持久化...{messages.Count}");
                    await this.dbConnection.ExecuteAsync(insertsql, messages, tran);
                    await this.dbConnection.ExecuteAsync(inserttwosql, messages, tran);
                    await this.dbConnection.ExecuteAsync(updatesql, new
                    {
                        ids = messages.Select(p => p.Id)
                    }, tran);

                    await tran.CommitAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Err:{0}", ex.Message);
                    tran.Rollback();
                }

            

        }

        public override bool TryWrite(IList<Product> messages)
        {
            if (!messages.Any())
                return false;

            /*var result = this.dbConnection.Execute(updatesql, new
            {
                ids = messages.Select(p => p.Id)
            });*/

            try
            {
                WriterDatabase(messages).GetAwaiter().GetResult();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
           // return true;
        }

        public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default)
        {
            return cancellationToken.IsCancellationRequested ? new ValueTask<bool>(Task.FromCanceled<bool>(cancellationToken)) : new ValueTask<bool>(true);
        }
    }
}
