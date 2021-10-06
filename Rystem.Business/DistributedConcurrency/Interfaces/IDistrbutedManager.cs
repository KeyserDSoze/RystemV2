using System;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public interface IDistributedManager<TKey> : IWarmUp
        where TKey : IDistributedConcurrencyKey
    {
        Task<RaceConditionResponse> RunUnderRaceConditionAsync(TKey key, Func<Task> task, Installation installation = Installation.Default, TimeSpan timeWindow = default);
        Task<LockResponse> LockAsync(TKey key, Func<Task> task, Installation installation = Installation.Default);
    }
}