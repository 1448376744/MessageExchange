using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;
using System.Collections.Concurrent;

namespace ExchangeBus
{
    public class ReceiveEndpointBuilder
    {
        readonly IEventBusSubscriptionsManager _subscriptionsManager;
        
        public ReceiveEndpointBuilder(IEventBusSubscriptionsManager subscriptionsManager)
        {
            _subscriptionsManager = subscriptionsManager;
        }
        
        public void MapEndpoint<TEvent, TEventHandler>()
               where TEvent : IIntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            _subscriptionsManager.AddSubscription<TEvent, TEventHandler>();
        }

        public IEventBusSubscriptionsManager Build()
        {
            return _subscriptionsManager;
        }
    }
}
