using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeBus.RabbitMQ
{
    public class RabbitMQExchangeHostBuilder : ExchangeHostBuilder
    {
        internal JsonSerializerOptions JsonSerializerOptions;

        internal IEventBusSubscriptionsManager SubscriptionsManager;
        
        internal ConnectionFactory ConnectionFactory { get; } = new ConnectionFactory();

        public RabbitMQExchangeHostBuilder ConfigureConnectionFactory(Action<ConnectionFactory> configure)
        {
            configure(ConnectionFactory);
            return this;
        }

        public override ExchangeHostBuilder ConfigureJsonSerializerOptions(Action<JsonSerializerOptions> configure)
        {
            configure(JsonSerializerOptions);
            return this;
        }

        public override ExchangeHostBuilder UseEndpoints(Action<ReceiveEndpointBuilder> endpoints)
        {
            var builder = new ReceiveEndpointBuilder(new RabbitMQEventBusSubscriptionsManager());
            endpoints(builder);
            SubscriptionsManager = builder.Build();
            return this;
        }
    }
}
