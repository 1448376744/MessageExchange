using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public class RabbitMQEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        readonly Dictionary<string, List<SubscriptionInfo>> _subscriptions = new();

        public void AddSubscription<TEvent, TEventHandler>()
            where TEvent : IIntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            var key = GetEntryKey<TEvent>();
            var subscription = new SubscriptionInfo(typeof(TEvent), typeof(TEventHandler));
            if (_subscriptions.ContainsKey(key) && _subscriptions[key].Any(a => a.EventType == typeof(TEvent) && a.EventHandlerType == typeof(TEventHandler)))
            {
                _subscriptions[key].Add(subscription);
            }
            else
            {
                _subscriptions.Add(key, new List<SubscriptionInfo>
                {
                    subscription
                });
            }
        }

        public void RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : IIntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            var key = GetEntryKey<TEvent>();
            if (_subscriptions.ContainsKey(key))
            {
                var removeSubscription = _subscriptions[key]
                    .Where(a => a.EventType == typeof(TEvent) && a.EventHandlerType == typeof(TEventHandler))
                    .FirstOrDefault();
                _subscriptions[key].Remove(removeSubscription);
            }
        }

        private static string GetEntryKey<TEvent>()
        {
            return typeof(TEvent).Name;
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
        {
            if (_subscriptions.TryGetValue(eventName, out List<SubscriptionInfo> values))
            {
                return values;
            }
            return new List<SubscriptionInfo>();
        }

        public List<string> GetAllEventNames()
        {
            return _subscriptions.Keys.ToList();
        }
    }
}
