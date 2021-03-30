using ExchangeBus;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.UseRabbitMQHost(host =>
            {
                host.ConfigureConnectionFactory(c =>
                {
                    c.ClientProvidedName = "rabbitmq.Send";
                    c.HostName = "172.25.251.167";
                });
            });
            var provider = services.BuildServiceProvider();
            using (var scope = provider.CreateScope())
            {
                var exchanger = scope.ServiceProvider.GetRequiredService<IExchangerBus>();
                var input = string.Empty;
                do
                {
                    Console.Write("OrderID:");
                    input = Console.ReadLine();
                    if (input != "#")
                    {
                        exchanger.Publish(new CancelOrderEvent()
                        {
                            OrderId = input
                        });
                    }
                    else
                    {
                        break;
                    }

                } while (true);
            }
            Console.WriteLine("EXIT!!!");
        }
    }
}
