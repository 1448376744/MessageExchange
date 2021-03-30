using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public class SubscriptionEndpointsBuilder
    {
        readonly Type _consumerType;
        Type _eventType;

        readonly List<Type> _eventHandlerTypes = new ();

        public SubscriptionEndpointsBuilder(Type eventType)
        {
            _eventType = eventType;
        }
        
        public void AddSubscriptionBehavior<T>()
           where T : ISubscriptionBehavior
        {
            _eventType = typeof(T);
        }
        
        public void AddSubscription<H>()
        {
            _eventHandlerTypes.Add(typeof(H));
        }

        public SubscriptionInfo Build()
        {
            return new SubscriptionInfo(_consumerType, _eventType, _eventHandlerTypes);
        }
    }
}
