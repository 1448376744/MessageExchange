using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeBus.RabbitMQ
{
    public class RabbitMQExchangeHostBuilder
    {
        readonly JsonSerializerOptions _jsonSerializerOptions = new ();

        readonly RabbitMQEndpointsBuilder _rabbitMQReceiveEndpointBuilder = new ();

        readonly ConnectionFactory _connectionFactory = new ();

        public RabbitMQExchangeHostBuilder ConfigureConnectionFactory(Action<ConnectionFactory> configure)
        {
            configure(_connectionFactory);
            return this;
        }

        public RabbitMQExchangeHostBuilder ConfigureJsonSerializerOptions(Action<JsonSerializerOptions> configure)
        {
            configure(_jsonSerializerOptions);
            return this;
        }
        
        public void UseEndpoints(Action<RabbitMQEndpointsBuilder> eventBusSubscriptionsManager)
        {
            eventBusSubscriptionsManager(_rabbitMQReceiveEndpointBuilder);
        }
        
        public IEventBusSubscriptionsManager BuildEventBusSubscriptionsManager()
        {
            return _rabbitMQReceiveEndpointBuilder.Build();
        }

        public JsonSerializerOptions BuildJsonSerializerOptions()
        {
            return _jsonSerializerOptions;
        }

        public ConnectionFactory BuildConnectionFactory()
        {
            return _connectionFactory;
        }
    }
}
