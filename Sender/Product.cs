using Newtonsoft.Json;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sender
{
    public class Product:IMessage
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


    public class OutboxMessage
    {
        public string Id { get; set; }
        public string AggregateType { get; set; }
        public string AggregateId { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}
