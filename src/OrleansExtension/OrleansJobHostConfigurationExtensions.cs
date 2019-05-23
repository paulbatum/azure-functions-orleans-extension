using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    public static class OrleansJobHostConfigurationExtensions
    {
        public static IWebJobsBuilder AddOrleans(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }            
            
            builder.Services.AddSingleton<ISiloHostBuilder>(new SiloHostBuilder());            
            builder.Services.AddSingleton<OrleansActorTriggerBindingProvider>();
            builder.Services.AddHostedService<OrleansHostedService>();

            builder.AddExtension<OrleansExtension>();            
            return builder;
        }
    }
}
