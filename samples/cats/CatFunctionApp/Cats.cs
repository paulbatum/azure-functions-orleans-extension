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
using System.Diagnostics;

namespace CatFunctionApp
{
    public class CatEventMessage
    {
        public string CatName { get; set; }
        public string EventName { get; set; }
    }

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

        [FunctionName("CatQueueProcessor")]
        public static async Task ProcessCatEvent(
            [QueueTrigger("catqueue", Connection = "AzureWebJobsStorage")] CatEventMessage message,
            [Orleans] IClusterClient orleansClient,
            ILogger log)
        {
            var cat = orleansClient.GetGrain<ICatGrain>(message.CatName);

            switch (message.EventName)
            {
                case "Eat":
                    await cat.Eat();
                    break;
                default:
                    throw new ApplicationException($"Unrecognized event name: '{message.EventName}'.");
            }

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
