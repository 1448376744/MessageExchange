using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public interface IExchangeHost
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync();
    }
}
