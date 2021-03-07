using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BedrockServer
{
    public static class ObservableExtensions
    {
        public static IObservable<TSource> TakeWhile<TSource>(this IObservable<TSource> source, Func<TSource, bool> predicate)
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
