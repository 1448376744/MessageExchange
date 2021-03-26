using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MessageExchange
{
    public abstract class ExchangeHost : IExchangeHost
    {
        private ConcurrentDictionary<Type, Delegate> _messageHandlerDelegates 
            = new ConcurrentDictionary<Type, Delegate>();
       
        public abstract Task StartAsync(CancellationToken cancellationToken = default);

        public abstract Task StopAsync();
        
        internal Delegate CreateNotificationHandlerDelegate(object instance, Type messageType)
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
