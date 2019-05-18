using System;
using System.Threading.Tasks;
using Orleans;

namespace CatGrainInterfaces
{
    public interface ICatGrain : IGrainWithStringKey
    {
        Task Eat();

        Task<string> GetStatus();
    }
}
