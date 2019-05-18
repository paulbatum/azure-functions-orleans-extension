using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs.Extensions.Orleans;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(OrleansWebJobsStartup))]

namespace Microsoft.Azure.WebJobs.Extensions.Orleans
{
    public class OrleansWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddOrleans();
        }
    }
}
