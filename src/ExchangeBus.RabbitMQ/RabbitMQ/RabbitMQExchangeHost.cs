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
        readonly ConcurrentDictionary<SubscriptionInfo, Delegate> _eventHandleDelegates = new();
        readonly JsonSerializerOptions _jsonSerializerOptions;

        public RabbitMQExchangeHost(IServiceProvider provider,IEventBusSubscriptionsManager subscriptionsManager, JsonSerializerOptions jsonSerializerOptions)
        {
            _provider = provider;
            _subscriptionsManager = subscriptionsManager;
            _jsonSerializerOptions = jsonSerializerOptions;
        }


        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            using (var scope = _provider.CreateScope())
            {
                foreach (var eventName in _subscriptionsManager.GetAllEventNames())
                {
                    var channel = scope.ServiceProvider.GetRequiredService<RabbitMQChannel>();
                    var consumer = new AsyncEventingBasicConsumer(channel.Model);
                    channel.Model.BasicQos(0, 1, false);
                    consumer.Received += async (model, @event) =>
                    {
                        try
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                throw new OperationCanceledException("Cancelled");
                            }
                            var handlers = _subscriptionsManager.GetHandlersForEvent(eventName);
                            foreach (var item in handlers)
                            {
                                var handle = CreateNotificationHandlerDelegate(_provider, item)
                                    as Func<object, CancellationToken, Task>;
                                var request = JsonSerializer.Deserialize(@event.Body.Span, item.EventType, _jsonSerializerOptions);
                                await handle(request, cancellationToken);
                            }
                            channel.Model.BasicAck(@event.DeliveryTag, false);
                        }
                        catch
                        {
                            channel.Model.BasicNack(@event.DeliveryTag, false, true);
                            throw;
                        }
                    };
                    channel.Model.BasicConsume(queue: eventName, autoAck: false, consumer: consumer);
                }
                return _source.Task;
            }
        }
        public Task StopAsync()
        {
            _source.TrySetException(new OperationCanceledException("Cancelled"));
            return _source.Task;
        }
        protected Delegate CreateNotificationHandlerDelegate(IServiceProvider provider, SubscriptionInfo subscription)
        {
            return _eventHandleDelegates.GetOrAdd(subscription, t =>
            {
                var method = subscription.EventHandlerType.GetMethod(nameof(IIntegrationEventHandler<bool>.Handle), new Type[]
                {
                    subscription.EventType,
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
                    Expression.Constant(subscription.EventHandlerType),
                    Expression.Constant(Array.Empty<object>()));
                var parameter1Expression = Expression.Parameter(typeof(object), "@event");
                var instanceOfConvertExpression = Expression.Convert(handlerInstanceExpression,subscription.EventHandlerType);
                var eventConvertExpression = Expression.Convert(parameter1Expression, subscription.EventType);
                var parameter2Expression = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                var callExpression = Expression.Call(instanceOfConvertExpression, method, new Expression[]
                {
                     eventConvertExpression,
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
