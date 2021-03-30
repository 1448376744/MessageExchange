using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public class SubscriptionInfo
    {
        public string EventName { get; set; }
        public Type ConsumerType { get; set; }
        public Type EventType { get; private set; }
       
        public List<Type> EventHandlerTypes { get; private set; }

        public SubscriptionInfo(Type eventType, List<Type> eventHandlerTypes)
        {
            EventType = eventType;
            EventHandlerTypes = eventHandlerTypes;
        }
        
        public SubscriptionInfo(Type consumerType,Type eventType, List<Type> eventHandlerTypes)
        {
            EventType = eventType;
            EventHandlerTypes = eventHandlerTypes;
            ConsumerType = consumerType;
            EventName = eventType.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is SubscriptionInfo info &&
                   EventName == info.EventName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EventName);
        }
    }
}
