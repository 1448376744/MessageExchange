using MessageExchange;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Receive
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddExchangeHost(host =>
            {
                host.ConfigureConnectionFactory(c =>
                {
                    c.ClientProvidedName = "rabbitmq.Receive";
                    c.HostName = "192.168.76.110";
                });
                host.UseEndpoints(endpoints =>
                {
                    endpoints.MapEndpoints(System.Reflection.Assembly.GetExecutingAssembly());
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
