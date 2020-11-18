

using System;
using System.Buffers;
using System.Threading.Tasks;

namespace ReactiveSocket
{
    public interface ISocketClient : IDisposable
    {
        Task SendMessageAsync(byte[] message);
        Task SendMessageAsync(string message);
        IObservable<ReadOnlySequence<byte>> ReaderMessageObservable { get; }
     }
}