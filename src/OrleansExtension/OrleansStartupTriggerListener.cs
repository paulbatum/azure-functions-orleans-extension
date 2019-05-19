using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
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
using Orleans.Runtime;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    internal class OrleansStartupTriggerListener : IListener
    {
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly ILogger _logger;
        private readonly Assembly _grainAssembly;
        private readonly string _persistenceConnectionString;
        private bool _listening;

        public OrleansStartupTriggerListener(
            ITriggeredFunctionExecutor executor,
            Assembly grainAssembly,
            string persistenceConnectionString,
            ILogger logger)
        {
            _executor = executor;
            _logger = logger;
            _grainAssembly = grainAssembly;
            _persistenceConnectionString = persistenceConnectionString;
            _listening = false;
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
            _listening = true;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _listening = false;
            return Task.CompletedTask;
        }

        public async Task ExecuteAsync(IServiceProvider services, CancellationToken cancellationToken)
        {
            if(_listening)
            {
                await _executor.TryExecuteAsync(new TriggeredFunctionData { TriggerValue = services }, cancellationToken);
            }
        }
    }


}
