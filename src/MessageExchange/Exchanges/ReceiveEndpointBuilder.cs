using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;
using System.Collections.Concurrent;

namespace MessageExchange
{
    public abstract class ReceiveEndpointBuilder
    {
        protected readonly List<ReceiveEndpoint> endpoints = new();
        protected ConcurrentDictionary<Type, Delegate> _messageHandlerDelegates = new ConcurrentDictionary<Type, Delegate>();
        public ReceiveEndpointBuilder Map(string queueName, ReceiveEndpointDelegate request)
        {
            endpoints.Add(new ReceiveEndpoint(queueName, request));
            return this;
        }

        public ReceiveEndpointBuilder MapEndpoints(Assembly assembly)
        {
            var messageHandlerTypes = assembly.GetTypes()
                .Where(a => typeof(IMessageHandler).IsAssignableFrom(a))
                .Where(a => a.IsClass && !a.IsAbstract);
            foreach (var item in messageHandlerTypes)
            {
                MapReceiveEndpoint(item);
            }
            return this;
        }

        public abstract ReceiveEndpointBuilder MapReceiveEndpoint(Type messageHandlerType);

        public IReadOnlyList<ReceiveEndpoint> BuildEndpoints()
        {
            return endpoints.AsReadOnly();
        }

        protected Delegate CreateNotificationHandlerDelegate(object instance, Type messageType)
        {
            return _messageHandlerDelegates.GetOrAdd(instance.GetType(), t =>
            {
                var name = nameof(IMessageHandler<bool>.Handle);
                var method = t.GetMethod(name, new Type[]
                {
                    messageType,
                    typeof(CancellationToken)
                });
                var constantExpression = Expression.Constant(instance);
                var parameter1Expression = Expression.Parameter(typeof(IMessage), "message");
                var convertExpression = Expression.Convert(parameter1Expression, messageType);
                var parameter2Expression = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                var callExpression = Expression.Call(constantExpression, method, new Expression[]
                {
                     convertExpression,
                     parameter2Expression
                });
                var lambda = Expression.Lambda(callExpression, new ParameterExpression[]
                {
                     parameter1Expression,
                     parameter2Expression
                });
                return lambda.Compile();
            });
        }
    }
}
