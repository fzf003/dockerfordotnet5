using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderSercice.Services;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyService.ReactiveRabbit;

namespace OrderSercice
{
    public interface IRabbmitServiceWorker:IDisposable
    {
        IEndPoint<User> InputEndpoint { get; }
    }

    public class RabbmitServiceWorker : ServiceHostBase, IRabbmitServiceWorker
    {

        readonly ILogger<RabbmitServiceWorker> _logger;

        readonly IEndPoint<User> inputEndPoint;

        public RabbmitServiceWorker(IMessageBroker messageBroker, ILogger<RabbmitServiceWorker> logger)
           : base(messageBroker, logger)
        {
           
            ///服务接入端点
            inputEndPoint= messageBroker.GetServiceEndPoint<User>(topicName: OrderExchangeSetting.OrderExchange, topicType: ExchangeType.Direct, durable: true);

            this.RegisterCall(exchangeName: OrderExchangeSetting.OrderExchange, queueName: OrderExchangeSetting.OrderQueueName, routingKey: OrderExchangeSetting.OrderRoutingKey, onMessage: (context, properties) =>
            {
                var message = Encoding.UTF8.GetString(context.RequestMessage.Body.Span);
                logger.LogInformation($"{OrderExchangeSetting.OrderQueueName}:{message}");
                return Task.CompletedTask;
            });

            this.RegisterCall(exchangeName: OrderExchangeSetting.OrderExchange, queueName: OrderExchangeSetting.OrderDeleteQuueName, routingKey: $"{OrderExchangeSetting.OrderRoutingKey}-del", onMessage: (context, properties) =>
            {
                var message = Encoding.UTF8.GetString(context.RequestMessage.Body.Span);
                logger.LogInformation($"{OrderExchangeSetting.OrderDeleteQuueName}:{message}");
                return Task.CompletedTask;
            });
        }

        public IEndPoint<User> InputEndpoint => inputEndPoint;
 
    }
    
}
