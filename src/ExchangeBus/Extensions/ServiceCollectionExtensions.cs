using ExchangeBus;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatR(this IServiceCollection services,Action<MediatorBuilder> configure)
        {
            var builder = new MediatorBuilder(services);
            configure(builder);
            services.AddSingleton<IMediator,Mediator>();
            return services;
        }
    }
}
