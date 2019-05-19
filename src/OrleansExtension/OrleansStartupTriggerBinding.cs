using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    public class OrleansStartupTriggerBinding : ITriggerBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly ILogger _logger;
        private readonly Assembly _grainAssembly;
        private readonly ISiloHostBuilder _builder;
        private readonly string _persistenceConnectionString;
        private readonly IReadOnlyDictionary<string, Type> _emptyBindingContract = new Dictionary<string, Type>();
        private readonly IReadOnlyDictionary<string, object> _emptyBindingData = new Dictionary<string, object>();

        public OrleansStartupTriggerBinding(
            ParameterInfo parameter,
            Assembly grainAssembly,
            string persistenceConnectionString,
            ISiloHostBuilder builder,
            ILogger logger)
        {
            _parameter = parameter;            
            _grainAssembly = grainAssembly;
            _persistenceConnectionString = persistenceConnectionString;
            _builder = builder;
            _logger = logger;
        }

        public Type TriggerValueType => typeof(IServiceProvider);

        public IReadOnlyDictionary<string, Type> BindingDataContract
        {
            get { return _emptyBindingContract; }
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            // ValueProvider is via binding rules. 
            return Task.FromResult<ITriggerData>(new TriggerData(null, _emptyBindingData));
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            var listener = new OrleansStartupTriggerListener(context.Executor, _grainAssembly, _persistenceConnectionString, _logger);

            _builder
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(_grainAssembly).WithReferences())
                .AddStartupTask((services, cancellationToken) => listener.ExecuteAsync(services, cancellationToken))
                .AddAzureTableGrainStorage("TableStore", options => options.ConnectionString = _persistenceConnectionString);

            return Task.FromResult<IListener>(listener);
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new TriggerParameterDescriptor()
            {
                Name = _parameter.Name,
                Type = "OrleansStartupTrigger"
            };
        }
    }
}
