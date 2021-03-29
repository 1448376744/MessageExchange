using System.Threading;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public interface IIntegrationEventHandler
    {

    }

    public interface IIntegrationEventHandler<TEvent> : IIntegrationEventHandler
    {
        Task Handle(TEvent @event, CancellationToken cancellationToken);
    }
}
