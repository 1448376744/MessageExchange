using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchange.RabbitMQ
{
    internal class ChannelPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        readonly IConnection _connection;

        public ChannelPooledObjectPolicy(RabbitMQConnection connection)
        {
            _connection = connection.Connection;
        }

        public IModel Create()
        {
            return _connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj != null && !obj.IsClosed)
            {
                return true;
            }
            return false;
        }
    }
}
