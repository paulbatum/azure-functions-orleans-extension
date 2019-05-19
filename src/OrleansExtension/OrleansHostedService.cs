using Microsoft.Extensions.Hosting;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{    
    public class OrleansHostedService : IHostedService
    {        
        private readonly ISiloHostBuilder _builder;
        private ISiloHost _siloHost;

        public OrleansHostedService(ISiloHostBuilder builder)
        {
            _builder = builder;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _siloHost = _builder.Build();
            await _siloHost.StartAsync(cancellationToken);            
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _siloHost?.StopAsync();
        }
    }
}
