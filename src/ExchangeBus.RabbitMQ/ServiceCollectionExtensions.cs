using ExchangeBus;
using ExchangeBus.RabbitMQ;
using Microsoft.Extensions.ObjectPool;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExchangeHost(this IServiceCollection services, Action<RabbitMQExchangeHostBuilder> configure)
        {
            var builder = new RabbitMQExchangeHostBuilder();
            configure(builder);
            services.AddSingleton(s =>
            {
                var connection = new RabbitMQConnection(builder.ConnectionFactory.CreateConnection());
                var providre = new DefaultObjectPoolProvider();
                return providre.Create(new ChannelPooledObjectPolicy(connection));
            });
            services.AddScoped<RabbitMQChannel>();
            services.AddScoped<IExchangerBus>(s =>
            {
                return new RabbitMQExchangerBus(s.GetRequiredService<RabbitMQChannel>(), builder.JsonSerializerOptions);
            });
            services.AddSingleton<IExchangeHost>(s =>
            {
                return new RabbitMQExchangeHost(s, builder.SubscriptionsManager, builder.JsonSerializerOptions);
            });
            return services;
        }
    }
}
