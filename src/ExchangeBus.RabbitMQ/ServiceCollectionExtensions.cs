using ExchangeBus;
using ExchangeBus.RabbitMQ;
using Microsoft.Extensions.ObjectPool;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseRabbitMQHost(this IServiceCollection services, Action<RabbitMQExchangeHostBuilder> configure)
        {
            var builder = new RabbitMQExchangeHostBuilder();
            configure(builder);
            var factory = builder.BuildConnectionFactory();
            var subscriptionsManager = builder.BuildEventBusSubscriptionsManager();
            var jsonSerializerOptions = builder.BuildJsonSerializerOptions();
            services.AddSingleton(s=> 
            {
                return new RabbitMQConnection(factory);
            });
            services.AddSingleton(s =>
            {
                var providre = new DefaultObjectPoolProvider();
                var connection = s.GetRequiredService<RabbitMQConnection>();
                return providre.Create(new ChannelPooledObjectPolicy(connection));
            });
            services.AddScoped<RabbitMQChannel>();
            services.AddScoped<IExchangerBus>(s =>
            {
                var channel = s.GetRequiredService<RabbitMQChannel>();
                return new RabbitMQExchangerBus(channel, jsonSerializerOptions);
            });
            services.AddSingleton<IExchangeHost>(s =>
            {
                return new RabbitMQExchangeHost(s, subscriptionsManager, jsonSerializerOptions);
            });
            return services;
        }
    }
}
