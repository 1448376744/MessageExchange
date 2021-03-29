using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System;

namespace ExchangeBus.RabbitMQ
{
    public class RabbitMQChannel : IDisposable
    {
        private bool _dispose = false;

        private IModel _model;

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
            if (_dispose)
            {
                return;
            }
            _dispose = true;
            _pool.Return(_model);
            GC.SuppressFinalize(this);
        }
    }
}
