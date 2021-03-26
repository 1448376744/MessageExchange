using MessageExchange;
using MessageExchange.Attributes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace rabbitmq.Receive
{
    [Queue(Name = "order")]
    public class CancelOrderMessage : IMessage
    {
        public string OrderId { get; set; }
    }

    public class CancelOrderMessageHandler :
       IMessageHandler<CancelOrderMessage>
    {
        public Task Handle(CancelOrderMessage request,CancellationToken cancellationToken)
        {
            Console.WriteLine("订单已取消：" + request.OrderId);
            return Task.CompletedTask;
        }
    }
}
