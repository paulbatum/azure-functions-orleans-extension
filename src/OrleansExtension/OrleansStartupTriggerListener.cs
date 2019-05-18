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

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    internal class OrleansStartupTriggerListener : IListener
    {
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly ILogger _logger;
        private readonly Assembly _grainAssembly;
        private readonly string _persistenceConnectionString;
        private ISiloHost _host;

        public OrleansStartupTriggerListener(
            ITriggeredFunctionExecutor executor,
            Assembly grainAssembly,
            string persistenceConnectionString,
            ILogger logger)
        {
            this._executor = executor;
            this._logger = logger;
            this._grainAssembly = grainAssembly;
            this._persistenceConnectionString = persistenceConnectionString;
        }

        public void Cancel()
        {
            StopAsync(CancellationToken.None).Wait();
        }

        public void Dispose()
        {
            _host.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            InitializeHost();
            await _host.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_host != null)
                await _host.StopAsync();
        }

        private void InitializeHost()
        {
            if (_host == null)
            {
                var builder = new SiloHostBuilder()
                   .UseLocalhostClustering()
                   .Configure<ClusterOptions>(options =>
                   {
                       options.ClusterId = "dev"; // TODO: pull these from the config
                       options.ServiceId = "AdventureApp";
                   })
                   .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                   .ConfigureApplicationParts(parts => parts.AddApplicationPart(_grainAssembly).WithReferences())
                   .AddAzureTableGrainStorage("TableStore", options => options.ConnectionString = _persistenceConnectionString)
                   .AddStartupTask((services, cancellationToken) =>
                   {
                       return _executor.TryExecuteAsync(new TriggeredFunctionData { TriggerValue = services }, cancellationToken);
                   });

                _host = builder.Build();
            }
        }

    }


}
