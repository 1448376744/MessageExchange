using System.Threading;
using System.Threading.Tasks;

namespace MessageExchange
{
    public interface IMessageHandler
    {

    }

    public interface IMessageHandler<TRequest> : IMessageHandler
    {
        Task Handle(TRequest request, CancellationToken cancellationToken);
    }
}
