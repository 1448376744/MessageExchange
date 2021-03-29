using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public class SubscriptionInfo
    {
        public Type EventType { get; private set; }
       
        public Type EventHandlerType { get; private set; }

        public SubscriptionInfo(Type eventType, Type eventHandlerType)
        {
            EventType = eventType;
            EventHandlerType = eventHandlerType;
        }

        public override bool Equals(object obj)
        {
            return obj is SubscriptionInfo info &&
                   EqualityComparer<Type>.Default.Equals(EventType, info.EventType) &&
                   EqualityComparer<Type>.Default.Equals(EventHandlerType, info.EventHandlerType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EventType, EventHandlerType);
        }
    }
}
