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
        bool _disposed = false;
        readonly IConnection _connection;
        readonly ConnectionFactory _factory;
        public IConnection Connection
        {
            get
            {
                if (_connection == null || !_connection.IsOpen)
                {
                    _connection?.Dispose();
                    return _factory.CreateConnection();
                }
                return _connection;
            }
        }

        public RabbitMQConnection(ConnectionFactory factory)
        {
            _factory = factory;
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
