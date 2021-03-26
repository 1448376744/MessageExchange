using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchange
{
    public class PipelineBuilder
    {
        readonly IServiceCollection _services;
        public PipelineBuilder(IServiceCollection services)
        {
            _services = services;
        }
        public PipelineBuilder UseMiddleware(Type middlewareType)
        {
            _services.AddTransient(typeof(IPipelineBehavior<,>), middlewareType);
            return this;
        }
    }
}
