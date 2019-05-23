using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    internal class OrleansActorTriggerListener : IListener
    {
        private readonly ITriggeredFunctionExecutor _functionExecutor;
        private readonly ILogger _logger;
        private readonly GrainExecutor _grainExecutor;
        private readonly string _grainType;

        public OrleansActorTriggerListener(
            ITriggeredFunctionExecutor functionExecutor,
            string grainType,
            ILogger logger,
            GrainExecutor grainExecutor)
        {
            this._functionExecutor = functionExecutor;
            this._grainType = grainType;
            this._logger = logger;
            this._grainExecutor = grainExecutor;
        }

        public void Cancel()
        {
            StopAsync(CancellationToken.None).Wait();
        }

        public void Dispose()
        {
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {            
            _grainExecutor.RegisterFunctionExecutor(_grainType, _functionExecutor);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

       
    }

}
