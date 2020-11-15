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

namespace PipeChannelService
{
    public interface ISocketClient
    {
        PipeReader Reader { get; }

        PipeWriter Writer { get;  }
    }
    public class SocketClient
    {

        

        public static IObservable<string> ReaderAsync(PipeReader pipeReader, CancellationToken cancellationToken = default)
        {
            return Observable.Create<string>((obser) =>
                NewThreadScheduler.Default.Schedule(async () =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            var readresult = await pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);

                            var buffer = readresult.Buffer;

                            if (buffer.Length <= 0)
                            {
                                continue;
                            }

                            while (ContainsLine(ref buffer, out ReadOnlySequence<byte> line))
                            {

                                ProcessLine(line, obser);
                            }
                            ////将指针移到下一条数据的头位置。
                            pipeReader.AdvanceTo(buffer.Start, buffer.End);

                            if (readresult.IsCompleted)
                            {
                                break;
                            }

                        }
                        catch (Exception ex)
                        {
                            await pipeReader.CompleteAsync(ex).ConfigureAwait(false);
                            obser.OnError(ex);
                        }
                    }

                    pipeReader.Complete();
                    obser.OnCompleted();
                })

             );
        }

        public static async Task OneWriteHelloAsync(PipeWriter writer, string message, CancellationToken cancellationToken = default)
        {
            byte[] helloBytes = Encoding.UTF8.GetBytes($"{message}\n");

            await writer.WriteAsync(helloBytes, cancellationToken).ConfigureAwait(false);

            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        static async Task WriteHelloAsync(PipeWriter writer, string message, CancellationToken cancellationToken = default)
        {
            // Request at least 5 bytes from the PipeWriter.
            Memory<byte> memory = writer.GetMemory(5);

            // Write directly into the buffer.
            int written = Encoding.UTF8.GetBytes(message.AsSpan(), memory.Span);

            // Tell the writer how many bytes were written.
            writer.Advance(written);

            await writer.FlushAsync(cancellationToken);
        }

        private static bool ContainsBytes(ref ReadOnlySpan<byte> buffer, in ReadOnlySpan<byte> line)
        {
            // line is now `ReadOnlySpan` so we can use efficient `IndexOf` method
            var result= buffer.IndexOf(line) >= 0;

            return result;
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

       

        static void ProcessLine(in ReadOnlySequence<byte> buffer, IObserver<string> observer)
        {
            foreach (var segment in buffer)
            {
                var message = Encoding.UTF8.GetString(buffer);
                observer.OnNext(message);
            }
        }

    }
}
