using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public interface ISubscriptionBehavior
    {
        void OnExecuteing(ISubscriptionExecuteingContext context);
        Task OnExecute(ISubscriptionExecuteContext context, Func<Task> next);
    }
}
