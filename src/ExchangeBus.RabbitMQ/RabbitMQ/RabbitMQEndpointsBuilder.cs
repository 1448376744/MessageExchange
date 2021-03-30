using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus.RabbitMQ
{
    public class RabbitMQEndpointsBuilder
    {
        readonly RabbitMQEventBusSubscriptionsManager eventBusSubscriptionsManager = new();

        public IEventBusSubscriptionsManager Build()
        {
            return eventBusSubscriptionsManager;
        }

        public void MapEndpoints<TEvent>(Action<SubscriptionEndpointsBuilder> endpoints)
            where TEvent : IIntegrationEvent
        {
            var builder = new SubscriptionEndpointsBuilder(typeof(TEvent));
            endpoints(builder);
            eventBusSubscriptionsManager.AddSubscription(builder.Build());
        }
    }
}
