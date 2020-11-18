using System;
using Microsoft.Extensions.Logging;
using TinyServer.ReactiveSocket;

namespace PipeChannelService
{
    public interface IMessageHandle<T>
    {
        void OnMessage(T message);

        void OnError(Exception ex);

        void OnCompleted();
    }

    public static class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> observable, IMessageHandle<T> endPoint)
        {
            return observable.Subscribe(endPoint.OnMessage, endPoint.OnError,endPoint.OnCompleted);
        }
    }

    public class ReplyHandler : IMessageHandle<SocketAcceptClient>
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception ex)
        {
            throw new NotImplementedException();
        }

        public void OnMessage(SocketAcceptClient message)
        {
            throw new NotImplementedException();
        }
    }

    public class ConsoleEndpoint : IMessageHandle<string>
    {
        readonly ILogger<ConsoleEndpoint> _logger;

        readonly ILoggerFactory loggerFactory;

        readonly ISocketClient _socketClient;

        public ConsoleEndpoint(ILoggerFactory loggerFactory, ISocketClient socketClient )
        {
            _logger = loggerFactory.CreateLogger<ConsoleEndpoint>();

            _socketClient = socketClient;
        }

        public void OnCompleted()
        {
            _socketClient.Dispose();
            _logger.LogInformation("客户端完成....");
        }

        public void OnError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            _logger.LogError(ex.Message);

            Console.ResetColor();
        }

        public void OnMessage(string message)
        {
            _logger.LogInformation(message);
            //_socketClient.SendMessageAsync($"{Guid.NewGuid().ToString("N")}--{message}".ToMessageBuffer());
        }
    }


}