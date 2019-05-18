using Microsoft.Azure.WebJobs.Extensions.Orleans;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;

namespace CatClient
{
    class Program
    {
        async static Task Main(string[] args)
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "AdventureApp";
                })
                .Build();

            await client.Connect();

            var proxy = client.GetGrain<IProxyGrain>(Guid.NewGuid());


            await proxy.SetState("3");
            await proxy.Call("getStatus", "");

            Console.WriteLine("Done making actor calls.");
            Console.ReadLine();
        }
    }
}
