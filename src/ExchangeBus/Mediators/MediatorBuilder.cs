using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeBus
{

    public class MediatorBuilder
    {
        readonly IServiceCollection _services;

        public MediatorBuilder(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Configure request handlers
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public MediatorBuilder AddRequests(Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(a => typeof(IRequestHandler).IsAssignableFrom(a))
                .Where(a => a.IsClass && !a.IsAbstract);
            foreach (var item in types)
            {
                AddRequest(item);
            }
            return this;
        }

        private MediatorBuilder AddRequest(Type requestHandlerType)
        {
            var inteface = requestHandlerType.GetInterfaces()
                  .Where(a => typeof(IRequestHandler)
                  .IsAssignableFrom(a))
                  .FirstOrDefault();
            _services.AddTransient(inteface, requestHandlerType);
            return this;
        }

        private MediatorBuilder AddNotification(Type notificationHandlerType)
        {
            var item = notificationHandlerType.GetInterfaces()
                 .Where(a => typeof(INotificationHandler)
                 .IsAssignableFrom(a)).FirstOrDefault();
            _services.AddTransient(item, notificationHandlerType);
            return this;
        }

        /// <summary>
        /// Configure notification handlers
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public MediatorBuilder AddNotifications(Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(a => typeof(INotificationHandler).IsAssignableFrom(a))
                .Where(a => a.IsClass && !a.IsAbstract);
            foreach (var item in types)
            {
                AddNotification(item);
            }
            return this;
        }

        /// <summary>
        /// Configure request pipeline
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public MediatorBuilder Configure(Action<PipelineBuilder> configure)
        {
            var builder = new PipelineBuilder(_services);
            configure(builder);
            _services.AddSingleton(typeof(IPipelineBuilder), builder);
            return this;
        }
    }
}
