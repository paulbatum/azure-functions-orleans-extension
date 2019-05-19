using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    [Extension("Orleans", "Orleans")]
    internal class OrleansExtension : IExtensionConfigProvider
    {
        private readonly IConfiguration _configuration;
        private readonly INameResolver _nameResolver;
        private readonly ISiloHostBuilder _siloHostBuilder;
        private readonly OrleansStartupTriggerBindingProvider _startupTriggerProvider;
        private readonly OrleansActorTriggerBindingProvider _actorTriggerProvider;
        private ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<Tuple<string, string>, IClusterClient> _clientCache = new ConcurrentDictionary<Tuple<string, string>, IClusterClient>();


        public OrleansExtension(
            ILoggerFactory loggerFactory, 
            IConfiguration configuration, 
            INameResolver nameResolver,
            OrleansStartupTriggerBindingProvider startupTriggerProvider,
            OrleansActorTriggerBindingProvider actorTriggerProvider,
            ISiloHostBuilder siloHostBuilder,
            IServiceProvider serviceProvider)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;
            _nameResolver = nameResolver;
            _startupTriggerProvider = startupTriggerProvider;
            _actorTriggerProvider = actorTriggerProvider;
            _siloHostBuilder = siloHostBuilder;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _siloHostBuilder
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev"; // TODO: pull these from the config
                    options.ServiceId = "AdventureApp";
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback);

            _logger = _loggerFactory.CreateLogger(LogCategories.CreateTriggerCategory("Orleans"));

            var inputRule = context.AddBindingRule<OrleansAttribute>();
            inputRule.BindToInput(new ClusterClientBuilder(this));

            
            var startupTriggerRule = context.AddBindingRule<OrleansStartupTriggerAttribute>();
            startupTriggerRule.BindToTrigger<IServiceProvider>(_startupTriggerProvider);

            var actorTriggerRule = context.AddBindingRule<OrleansActorTriggerAttribute>();
            actorTriggerRule.BindToTrigger<ActorCallData>(_actorTriggerProvider);
            actorTriggerRule.AddConverter<ActorCallData, string>(x => x.State);

        }

        private async Task<IClusterClient> GetClusterClient(OrleansAttribute attribute)
        {
            var resolvedClient = _clientCache.GetOrAdd(new Tuple<string, string>(attribute.ClusterId, attribute.ServiceId), (key) =>
            {
                var clientBuilder = new ClientBuilder();                

                var client = clientBuilder                
                    .UseLocalhostClustering()
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = key.Item1;
                        options.ServiceId = key.Item2;
                    })
                    //.ConfigureApplicationParts(parts => parts.AddApplicationPart(attribute.GrainInterface.Assembly).WithReferences())
                    .Build();                

                return client;
            });

            if (!resolvedClient.IsInitialized)
                await resolvedClient.Connect();

            return resolvedClient;
        }

        private class ClusterClientBuilder : IAsyncConverter<OrleansAttribute, IClusterClient>
        {
            OrleansExtension _provider;

            public ClusterClientBuilder(OrleansExtension provider)
            {
                _provider = provider;
            }

            public async Task<IClusterClient> ConvertAsync(OrleansAttribute input, CancellationToken cancellationToken)
            {
                return await _provider.GetClusterClient(input);
            }
        }

    }
}
