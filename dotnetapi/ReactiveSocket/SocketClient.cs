using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Reactive.Concurrency;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ReactiveSocket
{
  

    public class SocketClient : ISocketClient
    {
        readonly PipeReader _reader;

        readonly PipeWriter _writer;

        readonly ILoggerFactory _factory;
        readonly Socket _socket;

        readonly Heartbeat _heartbeat;

        readonly ILogger<SocketClient> _logger;

        public static ISocketClient CreateClient(IPEndPoint iPEndPoint, ILoggerFactory factory)
        {
            var clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(iPEndPoint);
            return new SocketClient(clientSocket, factory);
        }

        internal SocketClient(Socket socket, ILoggerFactory factory)
        {
            this._socket = socket;

            this._heartbeat=new Heartbeat(this._socket,factory);

            var networkStream=new NetworkStream(_socket); 

            this._reader = PipeReader.Create(networkStream);

            this._writer = PipeWriter.Create(networkStream);

            this._factory = factory;

            this._heartbeat.OnDisconnect+=(s)=>
            {
                _logger.LogDebug("结束连接的客户端");
                 this.Dispose();
            };

            this._heartbeat.Start();

            this._logger=this._factory.CreateLogger<SocketClient>();
             
        }
  

        public IObservable<ReadOnlySequence<byte>> ReaderMessageObservable =>
              Observable.Create<ReadOnlySequence<byte>>((observer) => ReaderSchedule(observer, CancellationToken.None));


        IDisposable ReaderSchedule(IObserver<ReadOnlySequence<byte>> observer, CancellationToken cancellationToken = default)
        {
            return NewThreadScheduler.Default.Schedule(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var readresult = await this._reader.ReadAsync(cancellationToken).ConfigureAwait(false);

                        var buffer = readresult.Buffer;

                        if (buffer.Length <= 0)
                        {
                            continue;
                        }

                        while (ContainsLine(ref buffer, out ReadOnlySequence<byte> line))
                        {
                            ProcessLine(line, observer);
                        }
                        ////将指针移到下一条数据的头位置。
                        this._reader.AdvanceTo(buffer.Start, buffer.End);

                        if (readresult.IsCompleted)
                        {
                            break;
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        //ShutDownSocket();
                        break;
                    }
                }

                this._reader?.Complete();
                observer.OnCompleted();
            });
        }
 
        static bool ContainsLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            SequencePosition? position = buffer.PositionOf((byte)'\n');

            if (position == null)
            {
                line = default;
                return false;
            }

            line = buffer.Slice(0, position.Value);
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            return true;
        }



        static void ProcessLine(in ReadOnlySequence<byte> buffer, IObserver<ReadOnlySequence<byte>> observer)
        {
           // foreach (var segment in buffer)
            {
                observer.OnNext(buffer);
            }
        }

        public void Dispose()
        {
            _heartbeat?.Dispose();
            this._reader?.Complete();
            this._writer?.Complete();
            ShutDownSocket();
        }

          void ShutDownSocket()
        {
            this._socket?.Shutdown(SocketShutdown.Both);
            this._socket?.Close(1000);
            _logger.LogInformation("连接关闭完成....");
        }

        public Task SendMessageAsync(byte[] message)
        {
            return this._writer.SendAsync(message);
        }

        public Task SendMessageAsync(string message)
        {
            return this._writer.SendAsync(message.ToMessageBuffer());
        }

      
    }
}
