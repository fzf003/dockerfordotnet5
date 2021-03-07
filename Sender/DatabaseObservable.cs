using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sender
{
    public  class DatabaseObservable
    {
        public static IObservable<T> CreateFromSqlCommand<T>(string connectionString, string command, Func<DbDataReader, Task<T>> readDataFunc)
        {
            return CreateFromSqlCommand(connectionString, command, readDataFunc, CancellationToken.None);
        }

        public static IObservable<T> CreateFromSqlCommand<T>(string connectionString, string command, Func<DbDataReader, Task<T>> readDataFunc, CancellationToken cancellationToken)
        {

            return Observable.Create<T>(
                async o =>
                {
                    DbDataReader reader = null;

                    try
                    {
                        using (var conn = new SqlConnection(connectionString))
                        using (var cmd = conn.CreateCommand())
                        {
                            ///cmd.Transaction = conn.BeginTransaction();
                            cmd.CommandText = command;

                            await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                            reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken).ConfigureAwait(false);
                             
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                var data = await readDataFunc(reader);
                                o.OnNext(data);
                            }

                            o.OnCompleted();
                        }
                    }
                    catch (Exception ex)
                    {
                        o.OnError(ex);
                    }

                    return reader;
                });
        }

        public static IObservable<T> CreateQuerySqlCommand<T>(string connectionString, string command, CancellationToken cancellationToken)
        {
             return Observable.Create<T>(
                async o =>
                {
                    DbDataReader reader = null;
                    try
                    {
                        using (var conn = new SqlConnection(connectionString))
                        using (var cmd = new SqlCommand(command, conn))
                        {
                            await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                            reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken).ConfigureAwait(false);
                            var sqlparser = reader.GetRowParser<T>();
                            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                            {
                                var data = (T)sqlparser(reader);

                                o.OnNext(data);
                            }

                            o.OnCompleted();
                        }
                    }
                    catch (Exception ex)
                    {
                        o.OnError(ex);
                    }

                    return reader;
                });
        }
    }
}
