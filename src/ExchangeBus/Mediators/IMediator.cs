using System.Threading;
using System.Threading.Tasks;

namespace ExchangeBus
{
    public interface IMediator
    {
        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) 
            where TNotification : INotification;
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        Task Send(IRequest request, CancellationToken cancellationToken = default);
    }
}
