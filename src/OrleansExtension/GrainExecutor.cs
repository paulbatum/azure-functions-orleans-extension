using Microsoft.Azure.WebJobs.Host.Executors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    public class ActorCallData
    {
        public string GrainType { get; set; }
        public string State { get; set; }
        public string EventName { get; set; }
        public string Data { get; set; }
    }

    public interface IProxyGrainExecutor
    {
        Task ExecuteAsync(ActorCallData data);
    }

    internal class GrainExecutor : IProxyGrainExecutor
    {
        private readonly ConcurrentDictionary<string, ITriggeredFunctionExecutor> _registeredListeners
            = new ConcurrentDictionary<string, ITriggeredFunctionExecutor>();

        public void RegisterFunctionExecutor(string grainType, ITriggeredFunctionExecutor functionExecutor)
        {
            _registeredListeners.TryAdd(grainType, functionExecutor);
        }

        async Task IProxyGrainExecutor.ExecuteAsync(ActorCallData data)
        {
            if(_registeredListeners.TryGetValue(data.GrainType, out ITriggeredFunctionExecutor functionExecutor))
            {
                await functionExecutor.TryExecuteAsync(new TriggeredFunctionData
                {
                    TriggerValue = data
                }, CancellationToken.None);
            }
            else
            {
                throw new Exception($"Grain type: '{data.GrainType} not registered.");
            }
        }
    }
}
