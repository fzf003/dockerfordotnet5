using Ecommerce;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoService.Services
{
    public class OrderManagementService: OrderManagement.OrderManagementBase
    {
        public override Task processOrders(IAsyncStreamReader<StringValue> requestStream, IServerStreamWriter<CombinedShipment> responseStream, ServerCallContext context)
        {
            return base.processOrders(requestStream, responseStream, context);
        }
    }
}
