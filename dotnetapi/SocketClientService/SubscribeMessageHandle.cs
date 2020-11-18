using System;
using System.Reactive;
using System.Threading;
using Microsoft.Extensions.Logging;
using TinyServer.ReactiveSocket;
using MyUnit=System.Reactive.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SocketClientService
{
    public class SubscribeMessageHandle : IMessageHandle<string>
    {
         readonly ILoggerFactory _logfactory;

         readonly ILogger<SubscribeMessageHandle> _logger;

        public SubscribeMessageHandle(ILoggerFactory logfactory)
        {
            _logfactory = logfactory;

            _logger=logfactory.CreateLogger<SubscribeMessageHandle>();
        }

        public void OnCompleted()
        {
           _logger.LogInformation("订阅完成.....");
        }

        public void OnError(Exception ex)
        {
           _logger.LogError("订阅出错:{0}",ex.Message);
        }

        public void OnMessage(string message)
        {
            _logger.LogInformation("订阅接收:{0}",message);
        }
    }

    public class SendMessageHandle : IMessageHandle<MyUnit>
    {
       readonly ILoggerFactory _logfactory;

         readonly ILogger<SendMessageHandle> _logger;

         readonly IServiceProvider serviceProvider;

        public SendMessageHandle(ILoggerFactory logfactory,  IServiceProvider serviceProvider)
        {
            _logfactory = logfactory;

            _logger = logfactory.CreateLogger<SendMessageHandle>();
            
            this.serviceProvider = serviceProvider;
        }

        public void OnCompleted()
        {
           _logger.LogInformation("发送已完成.....");

           this.serviceProvider.GetService<IHostedService>().StartAsync(CancellationToken.None);
        }

        public void OnError(Exception ex)
        {
           _logger.LogError($"发送出错:{ex.Message}");
           this.serviceProvider.GetService<IHostedService>().StartAsync(CancellationToken.None);
        }

      

        public void OnMessage(MyUnit message)
        {
             
        }
    }


    

}