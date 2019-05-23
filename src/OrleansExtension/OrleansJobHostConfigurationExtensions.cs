using Microsoft.Extensions.Configuration;
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
            builder.Services.AddSingleton<GrainExecutor>();
            builder.Services.AddHostedService<OrleansHostedService>();

            builder.AddExtension<OrleansExtension>()
                .ConfigureOptions<OrleansOptions>((config, path, options) =>
                {
                    options.StorageProviderConnectionString = config.GetConnectionStringOrSetting("OrleansStorageProviderConnectionString");

                    IConfigurationSection section = config.GetSection(path);
                    section.Bind(options);
                });

            return builder;
        }
    }
}
