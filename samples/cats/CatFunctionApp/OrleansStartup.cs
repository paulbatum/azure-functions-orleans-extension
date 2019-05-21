using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;       
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Orleans;
using Orleans;
using Orleans.Hosting;

namespace CatFunctionApp
{
    public class OrleansStartup
    {
        [FunctionName("OrleansServerStartup")]
        public static async Task Startup(
            [OrleansStartupTrigger(GrainType = typeof(CatGrain), PersistenceConnectionStringSetting = "AzureWebJobsStorage")] IServiceProvider services)
        {
            var grainFactory = (IGrainFactory) services.GetService(typeof(IGrainFactory));

            var fatcat = grainFactory.GetGrain<ICatGrain>("fatcat");
            for (int i = 0; i < 20; i++)
                await fatcat.Eat();
        }
    }
}
