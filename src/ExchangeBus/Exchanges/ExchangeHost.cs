using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public abstract class ExchangeHost : IExchangeHost
    {
        public abstract Task StartAsync(CancellationToken cancellationToken = default);
        public abstract Task StopAsync();
    }
}
