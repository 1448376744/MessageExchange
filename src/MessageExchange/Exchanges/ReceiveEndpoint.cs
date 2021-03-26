using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchange
{
    public class ReceiveEndpoint
    {
        public string QueueName { get; private set; }
        public ReceiveEndpointDelegate EndpointDelegate { get; private set; }
        public ReceiveEndpoint(string queueName, ReceiveEndpointDelegate request)
        {
            QueueName = queueName;
            EndpointDelegate = request;
        }
    }
}
