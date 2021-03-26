using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageExchange
{
    public class ExchangeHostBuilder
    {
        public ReceiveEndpointBuilder ReceiveEndpointBuilder { get; internal set; }

        public JsonOptions JsonSerializerOptions { get; private set; } = new JsonOptions();

        public ExchangeHostBuilder UseEndpoints(Action<ReceiveEndpointBuilder> endpoints)
        {
            endpoints(ReceiveEndpointBuilder);
            return this;
        }

        public ExchangeHostBuilder ConfigureJsonOptions(Action<JsonOptions> configure)
        {
            configure(JsonSerializerOptions);
            return this;
        }
    }
}
