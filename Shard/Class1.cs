using Newtonsoft.Json;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shard
{
    public class OrderAccepted :
    IMessage
    {
        public Guid OrderId { get; set; }
    }

    public class OrderSubmitted :
    IEvent
    {
        public Guid OrderId { get; set; }
        public decimal Value { get; set; }
    }

    public class Product
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string ImageFile { get; set; }

        public decimal Price { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
