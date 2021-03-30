using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeBus.RabbitMQ;

namespace Receive
{
    public class AutoAckRabbitMQSubscriptionBehavior : RabbitMQSubscriptionBehavior
    {
        public override async Task OnExecute(RabbitMQSubscriptionExecuteContext context, Func<Task> next)
        {
            try
            {
                await next();
                context.BasicAck(false);
            }
            catch (Exception)
            {
                context.BasicNack(false,true);
                throw;
            }
        }

        public override void OnExecuteing(RabbitMQSubscriptionExecuteingContext context)
        {
            context.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            context.BasicConsume();
        }
    }
}
