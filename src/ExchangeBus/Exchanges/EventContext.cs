using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public class EventContext
    {
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
        public EventContext(byte[] body , CancellationToken cancellationToken)
        {
            Body = body;
            CancellationToken = cancellationToken;
        }
    }
}
