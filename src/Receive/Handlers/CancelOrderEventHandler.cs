using ExchangeBus;
using ExchangeBus.RabbitMQ.Attributes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace rabbitmq.Receive
{
    public class CancelOrderEvent : IIntegrationEvent
    {
        public string OrderId { get; set; }
    }

    [Consumer(Tag = "cancelOrderConsumer")]
    public class CancelOrderEventHandler :
       IIntegrationEventHandler<CancelOrderEvent>
    {
        public Task Handle(CancelOrderEvent request, CancellationToken cancellationToken)
        {
            Console.WriteLine("订单已取消：" + request.OrderId);
            return Task.CompletedTask;
        }
    }
}
