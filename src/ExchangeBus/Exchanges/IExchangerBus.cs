using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public interface IExchangerBus
    {
        void Publish(object @event);
        void Publish(string queue, byte[] bytes);
    }
}
 