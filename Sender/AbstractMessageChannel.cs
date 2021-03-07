using Shard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sender
{
    public class AbstractMessageChannel<T> : Channel<T>, IDisposable
    {
        readonly Channel<T> messagechannel;

        public AbstractMessageChannel(Channel<T> channel)
        {
            messagechannel = channel;

            this.Reader = messagechannel.Reader;

            this.Writer = messagechannel.Writer;

        }

        public IObservable<T> ReaderStream(CancellationToken cancellationToken = default)
        {
            return this.Reader.ToObservable(cancellationToken);
        }

        public Task WriterAsync(T message, CancellationToken cancellationToken = default)
        {
            return this.Writer.WriteAsync(message).AsTask();
        }


        public void Dispose()
        {
            this.Writer.Complete();
            
        }
    }
}
