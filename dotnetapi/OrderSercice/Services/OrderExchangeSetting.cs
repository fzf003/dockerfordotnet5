using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderSercice.Services
{
    public class OrderExchangeSetting
    {
        public const string OrderExchange = "Order.CreateTopic";

        public const string OrderQueueName = "CreateOrder";

        public const string OrderRoutingKey = "order.event";

        public const string OrderDeleteQuueName = "DelOrder";
    }
}
