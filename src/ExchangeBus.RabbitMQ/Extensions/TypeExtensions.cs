using ExchangeBus.RabbitMQ.Attributes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace ExchangeBus.RabbitMQ
{
    internal static class TypeExtensions
    {
        public static ConsumerAttribute GetConsumerAttribute(this Type messageType)
        {
            return (messageType.GetCustomAttributes(typeof(ConsumerAttribute), true).FirstOrDefault() as ConsumerAttribute) ?? new ConsumerAttribute
            {
                Tag = null
            };
        }
    }
}
