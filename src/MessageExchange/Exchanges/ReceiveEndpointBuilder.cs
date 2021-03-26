using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;
using MessageExchange.Attributes;

namespace MessageExchange
{
    public class ReceiveEndpointBuilder
    {
        readonly List<ReceiveEndpoint> endpoints = new();

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

        private ReceiveEndpointBuilder MapReceiveEndpoint(Type messageHandlerType)
        {
            var messageType = messageHandlerType.GetInterfaces()
                .Where(a => a.IsGenericType && typeof(IMessageHandler).IsAssignableFrom(a))
                .FirstOrDefault().GetGenericArguments()[0];
            var attribute = messageType.GetCustomAttributes(typeof(QueueAttribute), true)
                .FirstOrDefault() as QueueAttribute;
            var endpoint = new ReceiveEndpoint(attribute?.Name?? messageType.Name, async (context) =>
            {
                var host = context.ServiceProvider.GetRequiredService<IExchangeHost>() as ExchangeHost;
                var instance = ActivatorUtilities.GetServiceOrCreateInstance(context.ServiceProvider, messageHandlerType);
                var jsonOptions = context.ServiceProvider.GetRequiredService<JsonOptions>();
                var func = host.CreateNotificationHandlerDelegate(instance, messageType) as Func<IMessage, CancellationToken, Task>;
                var message = JsonSerializer.Deserialize(context.Body, messageType, jsonOptions.JsonSerializerOptions) as IMessage;
                await func(message, context.CancellationToken);
            });
            endpoints.Add(endpoint);
            return this;
        }

        public IReadOnlyList<ReceiveEndpoint> BuildEndpoints()
        {
            return endpoints.AsReadOnly();
        }
    }
}
