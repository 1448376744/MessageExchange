using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public abstract class ExchangeHostBuilder
    {        
        public abstract ExchangeHostBuilder UseEndpoints(Action<ReceiveEndpointBuilder> endpoints);

        public abstract ExchangeHostBuilder ConfigureJsonSerializerOptions(Action<JsonSerializerOptions> configure);
    }
}
