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
            foreach (var eventName in _subscriptionsManager.GetAllEventNames())
            {
                var channel = _provider.GetRequiredService<RabbitMQChannel>();
                StartBasicConsume(eventName, channel);
            }
            return _source.Task;
        }
       
        private void StartBasicConsume(string eventName, RabbitMQChannel channel)
        {
            var consumer = new AsyncEventingBasicConsumer(channel.Model);
            channel.Model.BasicQos(0, 1, false);
            consumer.Received += Consumer_ReceivedAsync;
            channel.Model.BasicConsume(queue: eventName, autoAck: false, consumer: consumer);
        }
      
        private async Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs @event)
        {
            IModel channel = sender as IModel;
            try
            {
                var handlers = _subscriptionsManager.GetHandlersForEvent(@event.RoutingKey);
                foreach (var item in handlers)
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var handle = CreateNotificationHandlerDelegate(scope.ServiceProvider, item) as Func<object, Task>;
                        var request = JsonSerializer.Deserialize(@event.Body.Span, item.EventType, _jsonSerializerOptions);
                        await handle(request);
                    }
                }
                channel.BasicAck(@event.DeliveryTag, false);
            }
            catch
            {
                channel.BasicNack(@event.DeliveryTag, false, true);
                throw;
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
