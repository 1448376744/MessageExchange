using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MessageExchange
{
    public class Mediator : IMediator
    {
        readonly IServiceProvider _provider;

        readonly ConcurrentDictionary<Type, Delegate> _handlerDelegates = new();

        readonly ConcurrentDictionary<Type, IEnumerable<Delegate>> _notificationHandlerDelegates = new();

        public Mediator(IServiceProvider provider)
        {
            _provider = provider;
        }

        public Task Send(IRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
            var instance = _provider.GetService(handlerType);
            var func = CreateRequestHandlerDelegate(instance, request.GetType()) as Func<IRequest, CancellationToken, Task>;
            //将无返回值的转换成返回bool的
            var pipeline = BuildPipeline<IRequest, bool>(async () =>
            {
                await func(request, cancellationToken);
                return true;
            });
            return pipeline(request, cancellationToken);
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var handlerType = typeof(IRequestHandler<,>)
                .MakeGenericType(request.GetType(), typeof(TResponse));
            var instance = _provider.GetService(handlerType);
            var func = CreateRequestHandlerDelegate(instance, request.GetType())
                as Func<IRequest, CancellationToken, Task<TResponse>>;
            var pipeline = BuildPipeline<IRequest<TResponse>, TResponse>(() => func(request, cancellationToken));
            return pipeline(request, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }
            var handlerType = typeof(INotificationHandler<>)
                .MakeGenericType(notification.GetType());
            var instances = _provider.GetService(typeof(IEnumerable<INotificationHandler<TNotification>>))
                as IEnumerable<INotificationHandler<TNotification>>;
            var funcs = CreateNotificationHandlerDelegate(instances, notification.GetType());
            var list = new List<Task>(funcs.Count());
            foreach (var item in funcs)
            {
                var func = item as Func<INotification, CancellationToken, Task>;
                list.Add(func(notification, cancellationToken));
            }
            return Task.Run(() =>
            {
                Task.WaitAll(list.ToArray());
            }, cancellationToken);
        }

        public PipelineDelegate<TRequest, TResponse> BuildPipeline<TRequest, TResponse>(RequestHandlerDelegate<TResponse> requestHandler)
           where TRequest : IRequest
        {
            var list = new List<Func<PipelineDelegate<TRequest, TResponse>, PipelineDelegate<TRequest, TResponse>>>();
            var middlewareType = typeof(IEnumerable<IPipelineBehavior<TRequest, TResponse>>);
            var middlewares = _provider.GetService(middlewareType)
                as IEnumerable<IPipelineBehavior<TRequest, TResponse>>;
            foreach (var handler in middlewares)
            {
                list.Add(next =>
                {
                    return (request, cancellationToken) =>
                    {
                        return handler.Handle(request, () => next(request, cancellationToken), cancellationToken);
                    };
                });
            }
            PipelineDelegate<TRequest, TResponse> app = (request, cancellationToken) =>
            {
                return requestHandler();
            };
            ;
            foreach (var item in list.AsEnumerable().Reverse())
            {
                app = item(app);
            }
            return app;
        }

        private Delegate CreateRequestHandlerDelegate(object instance, Type requestType)
        {
            return _handlerDelegates.GetOrAdd(requestType, t =>
             {
                 var name = nameof(IRequestHandler<IRequest<bool>, bool>.Handle);
                 var method = instance.GetType()
                     .GetMethod(name, new Type[] { requestType, typeof(CancellationToken) });
                 var instanceExpression = Expression.Constant(instance);
                 var parameter1Expression = Expression.Parameter(typeof(IRequest), "request");
                 var parameter2Expression = Expression.Parameter(typeof(CancellationToken), "request");
                 var convertExpression = Expression.Convert(parameter1Expression, requestType);
                 var callExpression = Expression.Call(instanceExpression, method, convertExpression, parameter2Expression);
                 var lambda = Expression.Lambda(callExpression, parameter1Expression, parameter2Expression);
                 return lambda.Compile();
             });
        }

        private IEnumerable<Delegate> CreateNotificationHandlerDelegate(IEnumerable<object> instances, Type requestType)
        {
            return _notificationHandlerDelegates.GetOrAdd(requestType, t =>
            {
                var list = new List<Delegate>();
                foreach (var instance in instances)
                {
                    var name = nameof(INotificationHandler<INotification>.Handle);
                    var method = instance.GetType()
                        .GetMethod(name, new Type[] { requestType, typeof(CancellationToken) });
                    var instanceExpression = Expression.Constant(instance);
                    var parameter1Expression = Expression.Parameter(typeof(INotification), "request");
                    var parameter2Expression = Expression.Parameter(typeof(CancellationToken), "request");
                    var convertExpression = Expression.Convert(parameter1Expression, requestType);
                    var callExpression = Expression.Call(instanceExpression, method, convertExpression, parameter2Expression);
                    var lambda = Expression.Lambda(callExpression, parameter1Expression, parameter2Expression);
                    list.Add(lambda.Compile());
                }
                return list;
            });
        }
    }
}
