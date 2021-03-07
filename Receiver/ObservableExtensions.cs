using System;
using System.Buffers;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;

namespace Receiver
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
           // channelWriter.WriteAsync()

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
 
    }
}
