using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus.RabbitMQ
{
    public class RabbitMQSubscriptionExecuteingContext : ISubscriptionExecuteingContext
    {
        readonly IModel _channel;
        readonly IBasicConsumer _consumer;
        readonly string _eventName;
        public RabbitMQSubscriptionExecuteingContext(IModel model, IBasicConsumer consumer,string eventName)
        {
            _channel = model;
            _consumer = consumer;
            _eventName = eventName;
        }
       
        public void BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            _channel.BasicQos(prefetchSize, prefetchCount, global);
        }

        public void BasicConsume(bool autoAck = false, string consumerTag = "", bool noLocal = false, bool exclusive = false, IDictionary<string, object> arguments = null)
        {
            _channel.BasicConsume(
                queue: _eventName, 
                autoAck: autoAck, 
                arguments:arguments,
                consumerTag:consumerTag,
                noLocal: noLocal,
                exclusive: exclusive,
                consumer: _consumer);
        }
    }
}
