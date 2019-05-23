using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using Orleans.Runtime;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    internal interface IOrleansStartup
    {
        void RegisterStartupTask(IStartupTask startupTask);
    }

    internal class OrleansStartupTriggerBindingProvider : ITriggerBindingProvider, IOrleansStartup
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ISiloHostBuilder _builder;    
        

        public OrleansStartupTriggerBindingProvider(ILoggerFactory loggerFactory, IConfiguration configuration, ISiloHostBuilder siloHostBuilder)
        {
            _logger = loggerFactory.CreateLogger(LogCategories.CreateTriggerCategory("OrleansStartup"));
            _configuration = configuration;
            _builder = siloHostBuilder;
        }

        public void RegisterStartupTask(IStartupTask startupTask)
        {
            
            throw new NotImplementedException();
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
            
            return Task.FromResult<ITriggerBinding>(new OrleansStartupTriggerBinding(parameter, attribute.GrainType?.Assembly, resolvedSetting, _builder, _logger));
        }


    }
}
