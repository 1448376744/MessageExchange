using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus.RabbitMQ
{
    public class RabbitMQConnection : IDisposable
    {
        private bool _disposed = false;
        
        public IConnection Connection { get; }
        
        public RabbitMQConnection(IConnection connection)
        {
            Connection = connection;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            Connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
