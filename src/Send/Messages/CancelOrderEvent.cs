using MessageExchange;
using MessageExchange.RabbitMQ.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Send
{
    [Queue(Name = "order")]
    [Exchange(Name = "amq.fanout", Type = "fanout")]
    public class CancelOrderMessage : IMessage
    {
        public string OrderId { get; set; }
    }
}
