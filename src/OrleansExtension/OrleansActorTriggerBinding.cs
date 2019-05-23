using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    class OrleansActorTriggerBinding : ITriggerBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly ILogger _logger;
        private readonly IReadOnlyDictionary<string, Type> _bindingContract;
        private readonly IReadOnlyDictionary<string, object> _emptyBindingData = new Dictionary<string, object>();
        private readonly GrainExecutor _grainExecutor;
        private readonly string _grainType;

        public OrleansActorTriggerBinding(ParameterInfo parameter, string grainType, ILogger logger, GrainExecutor grainExecutor)
        {
            _parameter = parameter;
            _logger = logger;
            _grainExecutor = grainExecutor;
            _grainType = grainType;

            _bindingContract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                {"EventName", typeof(string) },
                {"Data", typeof(string) }
            };

        }

        public Type TriggerValueType => typeof(ActorCallData);

        public IReadOnlyDictionary<string, Type> BindingDataContract
        {
            get { return _bindingContract; }
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {            
            // ValueProvider is via binding rules. 
            return Task.FromResult<ITriggerData>(new TriggerData(null, GetBindingData((ActorCallData) value)));
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            return Task.FromResult<IListener>(new OrleansActorTriggerListener(context.Executor, _grainType, this._logger, _grainExecutor));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new TriggerParameterDescriptor()
            {
                Name = _parameter.Name,
                Type = "OrleansActorTrigger"
            };
        }

        private Dictionary<string, object> GetBindingData(ActorCallData actorCallData)
        {
            var bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            bindingData["EventName"] = actorCallData.EventName;
            bindingData["Data"] = actorCallData.Data;
            return bindingData;
        }
    }
}
