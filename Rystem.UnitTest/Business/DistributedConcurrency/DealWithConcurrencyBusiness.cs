using Rystem.Business;
using Rystem.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Business.DistributedConcurrency
{
    public class DealWithConcurrencyBusiness
    {
        public async Task RunASingleTimeOnTwoRaces(Installation installation)
        {
            async Task action() => await SumAsync(2, "3ad", installation);
            async Task action2() => await SumAsync(2, "3ad44", installation);
            async Task action3() => await SumAsync(2, "3ad", installation);
            await Task.WhenAll(action(), action2(), action3());
            await Task.Delay(100);
            Assert.Equal(4, Counter);
        }
        public async Task RunASingleTimeOnTwoRaces2(Installation installation)
        {
            async Task action() => await SumAsync2(2, "3ad", installation);
            async Task action2() => await SumAsync2(2, "3ad44", installation);
            async Task action3() => await SumAsync2(2, "3ad", installation);
            await Task.WhenAll(action(), action2(), action3());
            await Task.Delay(100);
            Assert.Equal(4, Counter);
        }
        private int Counter;
        private async Task SumAsync(int v, string key, Installation installation)
        {
            Func<Task> action = async () => await CountAsync(v);
            var dKey = new DealingKey() { Key = key };
            List<Task> tasks = new();
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(dKey.RunUnderRaceConditionAsync(action, installation));
            }
            await Task.WhenAll(tasks);
        }
        private async Task SumAsync2(int v, string key, Installation installation)
        {
            Func<Task> action = async () => await CountAsync(v);
            var dKey = new DealingKey() { Key = key };
            List<Task> tasks = new();
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(dKey.RunUnderRaceConditionAsync(action, installation));
            }
            await Task.WhenAll(tasks);
        }
        private async Task CountAsync(int v)
        {
            await Task.Delay(200);
            Counter += v;
        }
    }
}
