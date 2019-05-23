using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    public interface IProxyGrain : IGrainWithGuidKey
    {
        Task Call(string grainType, string eventName, string data);
    }

    public class ProxyGrain : Grain, IProxyGrain
    {
        private IProxyGrainExecutor _executor;
        private string _state;

        public ProxyGrain(IProxyGrainExecutor executor)
        {
            _executor = executor;
            _state = "default state";
        }

        public Task SetState(string state)
        {
            _state = state;
            return Task.CompletedTask;
        }

        public async Task Call(string grainType, string eventName, string data)
        {
            await _executor.ExecuteAsync(new ActorCallData
            {
                GrainType = grainType,
                State = _state,
                EventName = eventName,
                Data = data
            });
        }
    }
}
