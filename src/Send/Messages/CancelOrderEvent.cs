using ExchangeBus;
using ExchangeBus.RabbitMQ.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Send
{
    public class CancelOrderEvent : IIntegrationEvent
    {
        public string OrderId { get; set; }
    }
}
