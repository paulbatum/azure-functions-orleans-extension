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
    internal class OrleansActorTriggerListener : IListener, IProxyGrainExecutor
    {
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly ILogger _logger;        
        private ISiloHost _host;

        public OrleansActorTriggerListener(
            ITriggeredFunctionExecutor executor,
            ILogger logger)
        {
            this._executor = executor;
            this._logger = logger;            
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
                   .ConfigureServices(services =>
                   {
                       services.AddSingleton<IProxyGrainExecutor>(this);
                   })
                   .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ProxyGrain).Assembly));               

                _host = builder.Build();
            }
        }

        async Task IProxyGrainExecutor.ExecuteAsync(ActorCallData data)
        {
            await _executor.TryExecuteAsync(new TriggeredFunctionData
            {
                TriggerValue = data
            }, CancellationToken.None);
        }
    }

    public class ActorCallData
    {
        public string State { get; set; }
        public string EventName { get; set; }
        public string Data { get; set; }
    }

    public interface IProxyGrainExecutor
    {
        Task ExecuteAsync(ActorCallData data);
    }
}
