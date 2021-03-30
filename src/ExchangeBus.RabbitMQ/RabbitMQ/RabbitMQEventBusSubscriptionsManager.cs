using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public class RabbitMQEventBusSubscriptionsManager: IEventBusSubscriptionsManager
    {
        readonly List<SubscriptionInfo> _subscriptions = new();

        public void AddSubscription(SubscriptionInfo subscription)
        {
            _subscriptions.Add(subscription);
        }

        public List<SubscriptionInfo> GetAllSubscriptions()
        {
            return _subscriptions;
        }
    }
}
