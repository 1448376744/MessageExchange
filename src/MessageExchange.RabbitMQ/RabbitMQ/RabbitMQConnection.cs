using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchange.RabbitMQ
{
    public class RabbitMQConnection : IDisposable
    {
        public IConnection Connection { get; }
        
        public RabbitMQConnection(IConnection connection)
        {
            Connection = connection;
        }

        public void Dispose()
        {
            Connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
