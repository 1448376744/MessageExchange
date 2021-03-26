using MessageExchange.Attributes;
using System.Text.Json;
using System;
using System.Linq;

namespace MessageExchange.RabbitMQ
{
    public class RabbitMQExchanger : IExchanger
    {
        readonly RabbitMQChannel _channel;
        readonly JsonOptions _jsonOptions;

        public RabbitMQExchanger(RabbitMQChannel channel,JsonOptions jsonOptions)
        {
            _channel = channel;
            _jsonOptions = jsonOptions;
        }

        public void Publish(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            var queuName = message.GetType()
                .GetCustomAttributes(typeof(QueueAttribute), true)
                .FirstOrDefault() as QueueAttribute;
            var bytes =  JsonSerializer.SerializeToUtf8Bytes(message,_jsonOptions.JsonSerializerOptions);
            Publish(queuName.Name, bytes);
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
