using MessageExchange;
using MessageExchange.RabbitMQ;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
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
                var connection = builder.ConnectionFactory.CreateConnection();
                return new RabbitMQConnection(connection);
            });
            services.AddSingleton(s =>
            {
                var connection = s.GetRequiredService<RabbitMQConnection>();
                var providre = new DefaultObjectPoolProvider();
                return providre.Create(new ChannelPooledObjectPolicy(connection));
            });
            services.AddScoped<RabbitMQChannel>();
            services.AddScoped<IExchanger,RabbitMQExchanger>();
            services.AddSingleton<JsonOptions>();
            services.AddSingleton<IExchangeHost>(s =>
            {
                return new RabbitMQExchangeHost(builder, s);
            });
            return services;
        }
    }
}
