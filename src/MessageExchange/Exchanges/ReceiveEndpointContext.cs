using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageExchange
{
    public class ReceiveEndpointContext
    {
        /// <summary>
        /// scope ServiceProvider
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }
        
        /// <summary>
        /// message
        /// </summary>
        public byte[] Body { get; private set; }

        /// <summary>
        /// cancellationToken
        /// </summary>
        public CancellationToken CancellationToken { get; private set; }

        /// <summary>
        /// message context
        /// </summary>
        /// <param name="body"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="provider"></param>
        public ReceiveEndpointContext(byte[] body , IServiceProvider provider, CancellationToken cancellationToken)
        {
            Body = body;
            ServiceProvider = provider;
            CancellationToken = cancellationToken;
        }
    }
}
