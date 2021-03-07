using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AkkaServer.Core
{
    public class ActorRefProvider<T> : ActorRefProvider
    {
        public ActorRefProvider(IActorRef actorRef)
            : base(actorRef)
        {
        }
    }

    public abstract class ActorRefProvider
    {
        public IActorRef ActorRef { get; }
        protected ActorRefProvider(IActorRef actorRef)
        {
            ActorRef = actorRef;
        }
    }


    public static class ActorRefProviderExtensions
    {
        public static void Tell(this ActorRefProvider provider, object message, IActorRef sender)
        {
            provider.ActorRef.Tell(message, sender);
        }

        public static void Tell(this ActorRefProvider provider, object message)
        {
            provider.ActorRef.Tell(message, ActorRefs.NoSender);
        }

        public static Task<object> Ask(this ActorRefProvider provider, object message, TimeSpan? timeout = null)
        {
            return provider.ActorRef.Ask(message, timeout);
        }

        public static Task<object> Ask(this ActorRefProvider provider, object message, CancellationToken cancellationToken)
        {
            return provider.ActorRef.Ask(message, cancellationToken);
        }

        public static Task<object> Ask(this ActorRefProvider provider, object message, TimeSpan? timeout, CancellationToken cancellationToken)
        {
            return provider.ActorRef.Ask(message, timeout, cancellationToken);
        }

        public static Task<T> Ask<T>(this ActorRefProvider provider, object message, TimeSpan? timeout = null)
        {
            return provider.ActorRef.Ask<T>(message, timeout);
        }

        public static Task<T> Ask<T>(this ActorRefProvider provider, object message, CancellationToken cancellationToken)
        {
            return provider.ActorRef.Ask<T>(message, cancellationToken);
        }

        public static Task<T> Ask<T>(this ActorRefProvider provider, object message, TimeSpan? timeout, CancellationToken cancellationToken)
        {
            return provider.ActorRef.Ask<T>(message, timeout, cancellationToken);
        }

        public static Task<T> Ask<T>(this ActorRefProvider provider, Func<IActorRef, object> messageFactory, TimeSpan? timeout, CancellationToken cancellationToken)
        {
            return provider.ActorRef.Ask<T>(messageFactory, timeout, cancellationToken);
        }
    }
}
