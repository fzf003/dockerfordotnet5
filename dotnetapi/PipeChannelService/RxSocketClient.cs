using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipeChannelService
{
    public interface IRxSocketClient
    {
        bool Connected { get; }
        void Send(byte[] buffer);
        void Send(byte[] buffer, int offset, int length);
        IAsyncEnumerable<byte> ReadAsync(CancellationToken ct = default);
        IObservable<byte> ReceiveObservable { get; }
        Task DisposeAsync();
    }

     
}
