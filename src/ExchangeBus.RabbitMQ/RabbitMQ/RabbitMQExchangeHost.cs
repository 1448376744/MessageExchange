using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeBus.RabbitMQ
{
    public class RabbitMQExchangeHost : IExchangeHost
    {
        readonly TaskCompletionSource<int> _source = new();
        readonly IServiceProvider _provider;
        readonly IEventBusSubscriptionsManager _subscriptionsManager;
        readonly ConcurrentDictionary<Type, Delegate> _eventHandleDelegates = new();
        readonly JsonSerializerOptions _jsonSerializerOptions;

        public RabbitMQExchangeHost(IServiceProvider provider, IEventBusSubscriptionsManager subscriptionsManager, JsonSerializerOptions jsonSerializerOptions)
        {
            _provider = provider;
            _subscriptionsManager = subscriptionsManager;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            foreach (var subscription in _subscriptionsManager.GetAllSubscriptions())
            {
                StartBasicConsume(subscription);
            }
            return _source.Task;
        }

        private void StartBasicConsume(SubscriptionInfo subscription)
        {
            var channel = _provider.GetRequiredService<RabbitMQChannel>();
            ISubscriptionBehavior subscriptionBehavior = null;
            if (subscription.ConsumerType != null)
            {
                subscriptionBehavior = ActivatorUtilities.CreateInstance(_provider, subscription.ConsumerType) as ISubscriptionBehavior;
            }
            var consumer = new AsyncEventingBasicConsumer(channel.Model);
            consumer.Received += async (sender, eventArgs) =>
            {
                var model = sender as IModel;
                foreach (var item in subscription.EventHandlerTypes)
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var handle = CreateNotificationHandlerDelegate(scope.ServiceProvider, subscription.EventType, item) as Func<object, Task>;
                        var request = JsonSerializer.Deserialize(eventArgs.Body.Span, subscription.EventType, _jsonSerializerOptions);
                        await subscriptionBehavior?.OnExecute(new RabbitMQSubscriptionExecuteContext(model, eventArgs), () => handle(request));
                    }
                }
            };
            subscriptionBehavior?.OnExecuteing(new RabbitMQSubscriptionExecuteingContext(channel.Model, consumer, subscription.EventName));
            if (subscriptionBehavior == null)
            {
                channel.Model.BasicConsume(queue: subscription.EventName, consumer: consumer);
            }
        }


        public Task StopAsync()
        {
            _source.TrySetException(new OperationCanceledException("Cancelled"));
            return _source.Task;
        }

        protected Delegate CreateNotificationHandlerDelegate(IServiceProvider provider, Type eventType, Type eventHandlerType)
        {
            return _eventHandleDelegates.GetOrAdd(eventHandlerType, t =>
            {
                var method = eventHandlerType.GetMethod(nameof(IIntegrationEventHandler<bool>.Handle), new Type[]
                {
                    eventType,
                    typeof(CancellationToken)
                });
                var createInstanceMethod = typeof(ActivatorUtilities).GetMethod(nameof(ActivatorUtilities.CreateInstance), new Type[]
                {
                    typeof(IServiceProvider),
                    typeof(Type),
                    typeof(object[])
                });
                var handlerInstanceExpression = Expression.Call(
                    null,
                    createInstanceMethod,
                    Expression.Constant(provider),
                    Expression.Constant(eventHandlerType),
                    Expression.Constant(Array.Empty<object>()));
                var parameter1Expression = Expression.Parameter(typeof(object), "@event");
                var instanceOfConvertExpression = Expression.Convert(handlerInstanceExpression, eventHandlerType);
                var eventConvertExpression = Expression.Convert(parameter1Expression, eventType);
                var callExpression = Expression.Call(instanceOfConvertExpression, method, new Expression[]
                {
                     eventConvertExpression
                });
                var lambda = Expression.Lambda(callExpression, new ParameterExpression[]
                {
                     parameter1Expression
                });
                return lambda.Compile();
            });
        }
    }
}
