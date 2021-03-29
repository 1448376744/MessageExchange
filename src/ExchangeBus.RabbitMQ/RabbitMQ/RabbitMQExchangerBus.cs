using System.Text.Json;
using System;
using System.Linq;
using ExchangeBus.RabbitMQ.Attributes;

namespace ExchangeBus.RabbitMQ
{
    public class RabbitMQExchangerBus : IExchangerBus
    {
        readonly RabbitMQChannel _channel;
        readonly JsonSerializerOptions _jsonSerializerOptions;

        public RabbitMQExchangerBus(RabbitMQChannel channel, JsonSerializerOptions jsonSerializerOptions)
        {
            _channel = channel;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        public void Publish(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            var bytes =  JsonSerializer.SerializeToUtf8Bytes(message, _jsonSerializerOptions);
            Publish(message.GetType().Name, bytes);
        }

        public void Publish(string queue, byte[] bytes)
        {
            var body = new ReadOnlyMemory<byte>(bytes);
            _channel.Model.BasicPublish(
                exchange: "",
                routingKey: queue,
                mandatory: false,
                basicProperties: null,
                body: body);
        }
    }
}
