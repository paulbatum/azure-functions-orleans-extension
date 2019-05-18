using System;
using System.Threading.Tasks;
using CatGrainInterfaces;
using Orleans.Providers;

namespace CatGrains
{
    public class CatState
    {
        public int TimesEaten { get; set; }
    }

    [StorageProvider(ProviderName = "TableStore")]
    public class CatGrain : Orleans.Grain<CatState>, ICatGrain
    {
        public Task Eat()
        {
            this.State.TimesEaten += 1;
            return base.WriteStateAsync();
        }

        public Task<string> GetStatus()
        {
            return Task.FromResult($"I have eaten {this.State.TimesEaten} time(s).");
        }
    }


}
