using ExchangeBus;
using Microsoft.Extensions.DependencyInjection;
using rabbitmq.Receive;
using System;
using System.Threading.Tasks;

namespace Receive
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.UseRabbitMQHost(host =>
            {
                host.ConfigureConnectionFactory(c =>
                {
                    c.ClientProvidedName = "rabbitmq.Receive";
                    c.HostName = "172.25.251.167";
                    c.DispatchConsumersAsync = true;
                });
                host.ConfigureJsonSerializerOptions(options =>
                {
                    options.AllowTrailingCommas = true;
                });
                host.UseEndpoints(endpoints =>
                {
                    endpoints.MapEndpoints<CancelOrderEvent>(bus =>
                    {
                        bus.AddSubscription<CancelOrderEventHandler>();
                        bus.AddSubscription<CancelOrderEventHandler>();
                        bus.AddSubscriptionBehavior<AutoAckRabbitMQSubscriptionBehavior>();
                    });
                    endpoints.MapEndpoints<CancelOrderEvent>(bus =>
                    {
                        bus.AddSubscription<CancelOrderEventHandler>();
                        bus.AddSubscription<CancelOrderEventHandler>();
                    });
                });
            });
            var provider = services.BuildServiceProvider();
            using (var scope = provider.CreateScope())
            {
                var host = scope.ServiceProvider
                    .GetRequiredService<IExchangeHost>();
                await host.StartAsync();
            }
            Console.WriteLine("EXIT!!!");
        }
    }
}
