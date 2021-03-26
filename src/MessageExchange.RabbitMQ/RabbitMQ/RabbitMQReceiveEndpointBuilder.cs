using MessageExchange.RabbitMQ.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MessageExchange.RabbitMQ
{
    public class RabbitMQReceiveEndpointBuilder: ReceiveEndpointBuilder
    {

        public override ReceiveEndpointBuilder MapReceiveEndpoint(Type messageHandlerType)
        {
            var messageType = messageHandlerType.GetInterfaces()
                .Where(a => a.IsGenericType && typeof(IMessageHandler).IsAssignableFrom(a))
                .FirstOrDefault().GetGenericArguments()[0];
            var attribute = messageType.GetCustomAttributes(typeof(QueueAttribute), true)
                .FirstOrDefault() as QueueAttribute;
            var endpoint = new ReceiveEndpoint(attribute?.Name ?? messageType.Name, async (context) =>
            {
                var instance = ActivatorUtilities.GetServiceOrCreateInstance(context.ServiceProvider, messageHandlerType);
                var jsonOptions = context.ServiceProvider.GetRequiredService<JsonOptions>();
                var func = CreateNotificationHandlerDelegate(instance, messageType) as Func<IMessage, CancellationToken, Task>;
                var message = JsonSerializer.Deserialize(context.Body, messageType, jsonOptions.JsonSerializerOptions) as IMessage;
                await func(message, context.CancellationToken);
            });
            endpoints.Add(endpoint);
            return this;
        }
    }
}
