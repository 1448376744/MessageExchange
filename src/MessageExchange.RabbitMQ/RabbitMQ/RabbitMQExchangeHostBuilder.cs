using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchange.RabbitMQ
{
    public class RabbitMQExchangeHostBuilder : ExchangeHostBuilder
    {
        internal ConnectionFactory ConnectionFactory { get; } = new ConnectionFactory();
       
        public RabbitMQExchangeHostBuilder ConfigureConnectionFactory(Action<ConnectionFactory> configure)
        {
            configure(ConnectionFactory);
            return this;
        }
    }
}
