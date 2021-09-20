using Rystem;
using Rystem.Business;
using Rystem.Concurrency;
using System.Threading.Tasks;

namespace System
{
    public static class DistributedConcurrencyExtensions
    {
        private static IDistributedManager<TKey> Manager<TKey>(this TKey entity)
            where TKey : IDistributedConcurrencyKey
            => ServiceLocator.GetService<IDistributedManager<TKey>>();
        /// <summary>
        /// Deal with concurrency and allow only one method to run. Other concurrent task will be dropped.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key">Distributed concurrency key</param>
        /// <param name="task">Action to run</param>
        /// <param name="installation"></param>
        /// <returns></returns>
        public static Task<RaceConditionResponse> RunUnderRaceConditionAsync<TKey>(this TKey key, Func<Task> task, TimeSpan timeWindow = default, Installation installation = Installation.Default)
            where TKey : IDistributedConcurrencyKey
            => key.Manager().RunUnderRaceConditionAsync(key, task, installation, timeWindow);
        /// <summary>
        /// Deal with concurrency and allow only one method
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key">Distributed concurrency key</param>
        /// <param name="task">Action to run</param>
        /// <param name="installation"></param>
        /// <returns></returns>
        public static Task<LockResponse> LockAsync<TKey>(this TKey key, Func<Task> task, Installation installation = Installation.Default)
           where TKey : IDistributedConcurrencyKey
            => key.Manager().LockAsync(key, task, installation);
    }
}