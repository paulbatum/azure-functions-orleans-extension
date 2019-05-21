using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Orleans;
using Orleans;
using Orleans.Providers;

namespace CatFunctionApp
{
    public interface ICatGrain : IGrainWithStringKey
    {
        Task Eat();
        Task<string> GetStatus();
    }

    [StorageProvider(ProviderName = "TableStore")]
    public class CatGrain : Orleans.Grain<int>, ICatGrain
    {
        [FunctionName("GetCatStatus")]
        public static async Task<IActionResult> GetCatStatus(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "cat/{catName}")] HttpRequest req,
            [Orleans] IClusterClient orleansClient,
            string catName,
            ILogger log)
        {
            var cat = orleansClient.GetGrain<ICatGrain>(catName);
            var status = await cat.GetStatus();
            return new OkObjectResult(status);
        }

        [FunctionName("CatEat")]
        public static async Task<IActionResult> CatEat(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "cat/{catName}/eat")] HttpRequest req,
            [Orleans] IClusterClient orleansClient,
            string catName,
            ILogger log)
        {
            var cat = orleansClient.GetGrain<ICatGrain>(catName);
            await cat.Eat();
            var status = await cat.GetStatus();
            return new OkObjectResult(status);
        }

        public static async Task ProcessCatEvent(
            [QueueTrigger()]
            ILogger log)
        {

        }

        public Task Eat()
        {
            this.State += 1;
            return base.WriteStateAsync();
        }

        public Task<string> GetStatus()
        {
            return Task.FromResult($"I have eaten {this.State} time(s).");
        }
    }

}
