using ExchangeBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus.RabbitMQ
{
    public abstract class RabbitMQSubscriptionBehavior : ISubscriptionBehavior
    {

        public abstract void OnExecuteing(RabbitMQSubscriptionExecuteingContext context);
        
        public abstract Task OnExecute(RabbitMQSubscriptionExecuteContext context, Func<Task> next);

        public Task OnExecute(ISubscriptionExecuteContext context, Func<Task> next)
        {
            return OnExecute(context as RabbitMQSubscriptionExecuteContext, next);
        }

        public void OnExecuteing(ISubscriptionExecuteingContext context)
        {
            OnExecuteing(context as RabbitMQSubscriptionExecuteingContext);
        }
    }
}
