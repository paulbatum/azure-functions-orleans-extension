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

            builder.AddExtension<OrleansExtensionConfigProvider>();           
            return builder;
        }
    }
}
