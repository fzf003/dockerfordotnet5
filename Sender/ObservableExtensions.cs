using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;

namespace Sender
{
    public static class ObservableExtensions
    {
        public static ChannelReader<T> AsChannelReader<T>(this IObservable<T> observable, int? maxBufferSize = null)
        {
            var channel = maxBufferSize != null ? Channel.CreateBounded<T>(maxBufferSize.Value) : Channel.CreateUnbounded<T>();

            var disposable = observable.Subscribe(
                                value => channel.Writer.TryWrite(value),
                                error => channel.Writer.TryComplete(error),
                                () => channel.Writer.TryComplete());

            channel.Reader.Completion.ContinueWith(task => disposable.Dispose());

            return channel.Reader;
        }

        public static IObservable<T> ToObservable<T>(this ChannelReader<T> channelReader, CancellationToken cancellation = default)
        {
            return Observable.Create<T>(async observer =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    await foreach (var item in channelReader.ReadAllAsync(cancellation))
                    {
                        observer.OnNext(item);
                    }

                    observer.OnCompleted();
                }
            });
        }


       

        public static IObservable<T> ToObservable<T>(this ChannelWriter<T> channelWriter, CancellationToken cancellation = default)
        {

            return Observable.Return<T>(default);
        }

        public static IObservable<T> ToReaderObservable<T>(this ChannelReader<T> channelReader, CancellationToken cancellation = default)
        {
            return Observable.Create<T>(async observer =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    await foreach (var item in channelReader.ReadAllAsync(cancellation))
                    {
                        observer.OnNext(item);
                    }

                    observer.OnCompleted();
                }
            });
        }




        public static IDisposable Writer(this IObservable<ReadOnlyMemory<byte>> observable, PipeWriter writer)
        {

            return observable.Select(value => writer.WriteAsync(value).AsTask())
                             .Concat()
                             .WriterObservable(p => p.IsCanceled || p.IsCompleted)
                             .Subscribe();
        }

        public static IDisposable WriterToDatabase(this IObservable<IList<Product>> observable, ChannelWriter<IList<Product>> writer)
        {
            return observable.Select(value => Observable.FromAsync(()=> writer.WriteAsync(value).AsTask()))
                            // .WriterObservable(p => p.IsCanceled || p.IsCompleted)
                             .Concat()
                             .Subscribe(p=> {
                             },ex=> {
                                 Console.WriteLine("ex:{0}",ex.Message);
                             },()=> {
                                 Console.WriteLine("Comple。。。。。");
                             });
        }


        public static IObservable<ReadOnlySequence<byte>> ReaderBytesSource(this PipeReader reader, CancellationToken cancellationToken = default)
        {
            return Observable.Create<ReadOnlySequence<byte>>(async observer =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await reader.ReadAsync(cancellationToken);

                    ReadOnlySequence<byte> buffer = result.Buffer;

                    observer.OnNext(buffer);

                    reader.AdvanceTo(buffer.Start, buffer.End);

                    if (result.IsCompleted || result.IsCanceled)
                    {
                        Console.WriteLine("读取完成.....");
                        break;
                    }
                }

                // reader.Complete();
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public static IObservable<string> ReaderSource(this PipeReader reader, CancellationToken cancellationToken = default)
        {
            return Observable.Create<string>(async observer =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await reader.ReadAsync(cancellationToken);

                    if (result.IsCompleted || result.IsCanceled)
                        break;

                    ReadOnlySequence<byte> buffer = result.Buffer;

                    while (TryReadLine(ref buffer, out ReadOnlySequence<byte> message))
                    {
                        observer.OnNext(Encoding.UTF8.GetString(message.ToArray()));
                    }

                    reader.AdvanceTo(buffer.Start, buffer.End);
                }

                reader.Complete();
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        static bool TryParseMessage(ref ReadOnlySequence<byte> buffer, out string message) =>
                     (message = Encoding.UTF8.GetString(buffer.ToArray())) != null;

        static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {

            // Look for a EOL in the buffer.
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

        public static IObservable<TSource> WriterObservable<TSource>(this IObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return Observable.Create<TSource>(
                observer => source.Subscribe(
                    item =>
                    {
                        observer.OnNext(item);
                        if (predicate(item))
                            observer.OnCompleted();
                    },
                    observer.OnError,
                    observer.OnCompleted
                    )
                );
        }


        public static IDisposable SinkToFile(this IObservable<ReadOnlySequence<byte>> observable, PipeWriter pipeWriter)
        {

            Func<ReadOnlySequence<byte>, IObservable<FlushResult>> convert = (ReadOnlySequence<byte> user) => Observable.FromAsync(() => pipeWriter.WriteAsync(user.ToArray()).AsTask());
            return observable.Select(convert)
                             .Concat()
                             .WriterObservable(p => p.IsCanceled || p.IsCompleted)
                             .Subscribe(_ => { }, ex =>
                             {
                                 pipeWriter.Complete(ex);
                                 Console.WriteLine("文件写入出错:{0}", ex.Message);
                             }, () =>
                             {
                                 pipeWriter.Complete();
                                 Console.WriteLine("WriterDone....");
                             });
        }

    }
}
