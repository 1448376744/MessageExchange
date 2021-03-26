using MessageExchange;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddExchangeHost(host =>
            {
                host.ConfigureConnectionFactory(c =>
                {
                    c.ClientProvidedName = "rabbitmq.Send";
                    c.HostName = "192.168.76.110";
                });
            });
            var provider = services.BuildServiceProvider();
            using (var scope = provider.CreateScope())
            {
                var exchanger = scope.ServiceProvider.GetRequiredService<IExchanger>();
                var input = string.Empty;
                do
                {
                    Console.Write("OrderID:");
                    input = Console.ReadLine();
                    if (input != "#")
                    {
                        exchanger.Publish(new CancelOrderMessage()
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
