using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchange.RabbitMQ
{
    public class RabbitMQChannel : IDisposable
    {
        IModel _model;

        readonly ObjectPool<IModel> _pool;
        
        public IModel Model
        {
            get
            {
                if (_model == null)
                {
                    _model = _pool.Get();
                }
                return _model;
            }
        }

        public RabbitMQChannel(ObjectPool<IModel> pool)
        {
            _pool = pool;
        }

        public void Dispose()
        {
            _pool.Return(_model);
            GC.SuppressFinalize(this);
        }
    }
}
