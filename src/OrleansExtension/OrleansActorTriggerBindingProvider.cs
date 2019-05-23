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
        private readonly GrainExecutor _grainExecutor;

        public OrleansActorTriggerBindingProvider(
            ILoggerFactory loggerFactory,
            GrainExecutor grainExecutor)
        {
            _logger = loggerFactory.CreateLogger(LogCategories.CreateTriggerCategory("OrleansActor"));
            _grainExecutor = grainExecutor;
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


            return Task.FromResult<ITriggerBinding>(new OrleansActorTriggerBinding(parameter, attribute.GrainType, _logger, _grainExecutor));
        }
    }
}
