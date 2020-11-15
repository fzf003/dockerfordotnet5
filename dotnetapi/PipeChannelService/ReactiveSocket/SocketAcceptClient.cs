using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipeChannelService.ReactiveSocket
{
    public class SocketAcceptClient : IDisposable
    {
        readonly Socket _socket;

        readonly NetworkStream _networkstream;

        readonly PipeReader _pipeReader;

        readonly PipeWriter _pipeWriter;

        readonly SocketAsyncEventArgs _socketAsyncEventArgs;

        readonly TimeSpan heartbeat = TimeSpan.FromMilliseconds(800);

        readonly IDisposable disposable;

        readonly SocketServer _listenerserver;

        readonly ILogger<SocketAcceptClient> _logger;

      

        public SocketAcceptClient(Socket socket, SocketServer listenerserver, ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<SocketAcceptClient>();

            this._socket = socket;

            this._networkstream = new NetworkStream(_socket);

            this._pipeReader = PipeReader.Create(_networkstream);

            this._pipeWriter = PipeWriter.Create(_networkstream);

            this._socketAsyncEventArgs = new SocketAsyncEventArgs();

            disposable = Observable.Timer(TimeSpan.Zero, heartbeat)
                                   .Subscribe(Heartbeat);

            this._listenerserver = listenerserver;


        }

        static void PrintError(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(exception.Message);

            Console.ResetColor();
        }

        void Heartbeat(long heartbeat)
        {
            var socketstate = this._socket.Poll(10, SelectMode.SelectRead);

            if (socketstate)
            {
                this._listenerserver.RemoveConnection(this);

                _logger.LogDebug($"{DateTime.Now}----{_socket.RemoteEndPoint}连接断开....");

                this.Dispose();
            }
            else
            {
                _logger.LogDebug($"{DateTime.Now}----{_socket.RemoteEndPoint}连接正常....");
            }
        }


        public IObservable<ReadOnlySequence<byte>> RevicedObservable=>
               Observable.Create<ReadOnlySequence<byte>>((observer) => ReaderSchedule(observer, CancellationToken.None));
        

        IDisposable ReaderSchedule(IObserver<ReadOnlySequence<byte>> observer, CancellationToken cancellationToken = default)
        {
            return NewThreadScheduler.Default.Schedule(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var readresult = await Reader.ReadAsync(cancellationToken).ConfigureAwait(false);

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
                        Reader.AdvanceTo(buffer.Start, buffer.End);

                        if (readresult.IsCompleted)
                        {
                            break;
                        }

                    }
                    catch (Exception ex)
                    {
                        await Reader.CompleteAsync(ex).ConfigureAwait(false);
                        observer.OnError(ex);
                        break;
                    }
                }

                Reader.Complete();
                observer.OnCompleted();
            });
        }

        public PipeReader Reader => this._pipeReader;

        public PipeWriter Writer => this._pipeWriter;

        public void Dispose()
        {
            this._pipeReader.Complete();
            this._pipeWriter.Complete();
            disposable.Dispose();
            ShutDownSocket(_socket);
        }

        void ShutDownSocket(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close(1000);
            _logger.LogInformation("连接关闭完成....");
        }

        static bool ContainsLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            SequencePosition? position = buffer.PositionOf((byte)'\n');

            if (position == null)
            {
                line = default;
                return false;
            }

            // Skip the line + the \n.
            line = buffer.Slice(0, position.Value);
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            return true;
        }


        static void ProcessLine(in ReadOnlySequence<byte> buffer, IObserver<ReadOnlySequence<byte>> observer)
        {
            observer.OnNext(buffer);
        }
    }


}
