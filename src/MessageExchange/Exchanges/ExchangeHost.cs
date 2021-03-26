using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MessageExchange
{
    public abstract class ExchangeHost : IExchangeHost
    {
       
       
        public abstract Task StartAsync(CancellationToken cancellationToken = default);

        public abstract Task StopAsync();
        
       
    }
}
