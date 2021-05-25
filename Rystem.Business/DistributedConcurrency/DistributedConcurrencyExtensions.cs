using Rystem.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public static class DistributedConcurrencyExtensions
    {
        private static DistributedManager<TKey> Manager<TKey>(this TKey entity)
            where TKey : IDistributedConcurrencyKey
            => entity.DefaultManager(nameof(DistributedConcurrencyExtensions), (key) => new DistributedManager<TKey>(entity.BuildDistributed())) as DistributedManager<TKey>;
        /// <summary>
        /// Deal with concurrency and allow only one method to run. Other concurrent task will be dropped.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key">Distributed concurrency key</param>
        /// <param name="task">Action to run</param>
        /// <param name="installation"></param>
        /// <returns></returns>
        public static async Task<RaceConditionResponse> RunUnderRaceConditionAsync<TKey>(this TKey key, Func<Task> task, Installation installation = Installation.Default, TimeSpan timeWindow = default)
            where TKey : IDistributedConcurrencyKey
            => await task.RunUnderRaceConditionAsync(key.Key, timeWindow, key.Manager().Implementation(installation)).NoContext();
        /// <summary>
        /// Deal with concurrency and allow only one method
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="task"></param>
        /// <param name="installation"></param>
        /// <returns></returns>
        public static async Task<LockResponse> LockAsync<TKey>(this TKey key, Func<Task> task, Installation installation = Installation.Default)
           where TKey : IDistributedConcurrencyKey
            => await task.LockAsync(key.Key, key.Manager().Implementation(installation)).NoContext();
    }
}