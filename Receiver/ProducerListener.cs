using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receiver
{
    public class ProducerListener
    {
         readonly IScheduler _scheduler;

        private readonly IObservable<string> _messages;

        public ProducerListener()
        {
            _scheduler = new EventLoopScheduler();

            var messages = ListenToMessages()
                                   .SubscribeOn(_scheduler)
                                   .Publish();

            _messages = messages;
            messages.Connect();
        }

        public IObservable<string> Messages
        {
            get { return _messages; }
        }


        static async Task<DbConnection> ConnectionBuilder()
        {
            var sqlConnection = new SqlConnection("");
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

        private IObservable<string> ListenToMessages()
        {
            return Observable.Create<string>(o =>
            {
                return _scheduler.Schedule(recurse =>
                {
                    try
                    {
                        var messages = GetMessages();
                        foreach (var msg in messages)
                        {
                            o.OnNext(msg);
                        }
                        //recurse();
                    }
                    catch (Exception ex)
                    {
                        o.OnError(ex);
                    }
                });
            });
        }

        private IEnumerable<string> GetMessages()
        {
            while (true)
            {
                yield return Guid.NewGuid().ToString("N");
            }
        }
    }
}
