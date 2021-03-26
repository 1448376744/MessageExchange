using System.Threading;
using System.Threading.Tasks;

namespace MessageExchange
{
    public interface IRequestHandler
    {

    }
    public interface IRequestHandler<in TRequest> : IRequestHandler
        where TRequest : IRequest
    {
        Task Handle(TRequest request);
    }
    public interface IRequestHandler<in TRequest, TResponse> : IRequestHandler
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
