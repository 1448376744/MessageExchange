using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageExchange
{
    public delegate Task<TResponse> PipelineDelegate<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest;
}
