using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageExchange.RabbitMQ
{
    public class RabbitMQExchangeHost : ExchangeHost
    {
        readonly TaskCompletionSource<int> _source = new();

        readonly IServiceScope _serviceScope;

        readonly RabbitMQExchangeHostBuilder _hostBuilder;
        
        public RabbitMQExchangeHost(RabbitMQExchangeHostBuilder hostBuilder,IServiceProvider provider)
        {
            _serviceScope = provider.CreateScope();
            _hostBuilder = hostBuilder;
        }

        public override Task StartAsync(CancellationToken cancellationToken = default)
        {
            var endpoints = _hostBuilder.ReceiveEndpointBuilder
                .BuildEndpoints();
            foreach (var endpoint in endpoints)
            {
                var channel = _serviceScope.ServiceProvider.GetRequiredService<RabbitMQChannel>();
                var consumer = new EventingBasicConsumer(channel.Model);
                channel.Model.BasicQos(0, 1, false);
                consumer.Received += async (sender, events) =>
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            throw new OperationCanceledException("Cancelled");
                        }
                        using (var scope = _serviceScope.ServiceProvider.CreateScope())
                        {
                            var bytes = events.Body.ToArray();
                            var context = new ReceiveEndpointContext(bytes, scope.ServiceProvider, cancellationToken);
                            await endpoint.EndpointDelegate(context);
                            channel.Model.BasicAck(events.DeliveryTag, false);
                        }
                    }
                    catch
                    {
                        channel.Model.BasicNack(events.DeliveryTag, false, true);
                        throw;
                    }
                };
                channel.Model.BasicConsume(
                    queue: endpoint.QueueName,
                    autoAck: false,
                    consumer: consumer);

            }
            return _source.Task;
        }

        public override Task StopAsync()
        {
            _source.TrySetException(new OperationCanceledException("Cancelled"));
            _serviceScope?.Dispose();
            return _source.Task;
        }
    }
}
