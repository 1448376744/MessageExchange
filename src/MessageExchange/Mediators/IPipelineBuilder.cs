using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchange
{
    public interface IPipelineBuilder
    {
        void UseMiddleware(Type middlewareType);
        IReadOnlyList<Type> Build();
    }
}
