using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public interface IRequest
    {
        
    }
    public interface IRequest<out TResponse>: IRequest
    {

    }
}
