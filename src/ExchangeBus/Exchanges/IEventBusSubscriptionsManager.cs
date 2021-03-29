using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeBus
{
    public interface IEventBusSubscriptionsManager
    {
        void AddSubscription<TEvent, TEventHandler>()
            where TEvent : IIntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>;
       
        void RemoveSubscription<TEvent, TEventHandler>()
             where TEvent : IIntegrationEvent
             where TEventHandler : IIntegrationEventHandler<TEvent>;

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        List<string> GetAllEventNames();
        
    }
}
