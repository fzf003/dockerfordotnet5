using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AkkaServerApp
{
    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> ExecuteQuery(TQuery query);
    }

    public interface IQuery { }

    public interface IQuery<TResult> : IQuery { }

    public abstract class QueryHandler<TQuery, TResult> : ReceiveActor, IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        protected QueryHandler()
        {
            ReceiveAsync<TQuery>(async query =>
            {
                var result = await ExecuteQuery(query);
                Sender.Tell(result);
            });
        }

        public abstract Task<TResult> ExecuteQuery(TQuery query);
    }
}
