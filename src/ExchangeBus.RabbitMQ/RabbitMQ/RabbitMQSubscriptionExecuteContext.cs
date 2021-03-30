using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBus.RabbitMQ
{
    public class RabbitMQSubscriptionExecuteContext : ISubscriptionExecuteContext
    {
        readonly IModel _channel;

        readonly BasicDeliverEventArgs _eventArgs;
        
        public RabbitMQSubscriptionExecuteContext(IModel model, BasicDeliverEventArgs eventArgs)
        {
            _channel = model;
            _eventArgs = eventArgs;
        }
       
        public void BasicAck(bool multiple)
        {
            _channel.BasicAck(_eventArgs.DeliveryTag, multiple);
        }
        
        public void BasicReject(bool requeue)
        {
            _channel.BasicReject(_eventArgs.DeliveryTag, requeue);
        }
        
        public void BasicNack(bool multiple, bool requeue)
        {
            _channel.BasicNack(_eventArgs.DeliveryTag, multiple, requeue);
        }
    }
}
