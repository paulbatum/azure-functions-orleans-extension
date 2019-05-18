using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    class OrleansActorTriggerBindingProvider : ITriggerBindingProvider
    {
        private readonly ILogger _logger;

        public OrleansActorTriggerBindingProvider(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(LogCategories.CreateTriggerCategory("OrleansActor"));
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            ParameterInfo parameter = context.Parameter;
            var attribute = parameter.GetCustomAttribute<OrleansActorTriggerAttribute>(inherit: false);
            if (attribute == null)
            {
                // Is this the right pattern?
                return Task.FromResult<ITriggerBinding>(null);
            }


            return Task.FromResult<ITriggerBinding>(new OrleansActorTriggerBinding(parameter, _logger));
        }
    }
}
