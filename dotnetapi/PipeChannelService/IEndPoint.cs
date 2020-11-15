using System;

namespace PipeChannelService
{
    public interface IEndPoint<T>
    {
        void PushMessage(T message);

        void PushError(Exception ex);

    }

    public static class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> observable, IEndPoint<T> endPoint)
        {
            return observable.Subscribe(endPoint.PushMessage, endPoint.PushError);
        }
    }
}