using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace MessageExchange.RabbitMQ.Attributes
{
    public class ExchangeAttribute:Attribute
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
