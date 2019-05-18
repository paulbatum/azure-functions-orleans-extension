using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    internal class OrleansStartupTriggerBindingProvider : ITriggerBindingProvider
    {
        private readonly ILogger _logger;
        private IConfiguration _configuration;
        private INameResolver _nameResolver;        

        public OrleansStartupTriggerBindingProvider(ILoggerFactory loggerFactory, IConfiguration configuration, INameResolver nameResolver)
        {
            _logger = loggerFactory.CreateLogger(LogCategories.CreateTriggerCategory("OrleansStartup"));
            this._configuration = configuration;
            this._nameResolver = nameResolver;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            ParameterInfo parameter = context.Parameter;
            OrleansStartupTriggerAttribute attribute = parameter.GetCustomAttribute<OrleansStartupTriggerAttribute>(inherit: false);
            if (attribute == null)
            {
                // Is this the right pattern?
                return Task.FromResult<ITriggerBinding>(null);
            }

            var resolvedSetting = _configuration.GetConnectionStringOrSetting(attribute.PersistenceConnectionStringSetting);


            return Task.FromResult<ITriggerBinding>(new OrleansStartupTriggerBinding(parameter, attribute.GrainType?.Assembly, resolvedSetting, _logger));
        }
    }
}
