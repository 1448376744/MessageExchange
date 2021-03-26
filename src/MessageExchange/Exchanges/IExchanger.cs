using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchange
{
    public interface IExchanger
    {
        void Publish(object message);
        void Publish(string queue, byte[] bytes);
    }
}
 