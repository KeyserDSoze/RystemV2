using Rystem.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class DealWithConcurrency
    {
        [Fact]
        public async Task RunASingleTimeOnTwoRaces()
        {
            async Task action() => await SumAsync(2, "3ad");
            async Task action2() => await SumAsync(2, "3ad44");
            async Task action3() => await SumAsync(2, "3ad");
            await Task.WhenAll(action(), action2(), action3());
            await Task.Delay(100);
            Assert.Equal(4, Counter);
        }
        [Fact]
        public async Task RunASingleTimeOnTwoRaces2()
        {
            async Task action() => await SumAsync2(2, "3ad");
            async Task action2() => await SumAsync2(2, "3ad44");
            async Task action3() => await SumAsync2(2, "3ad");
            await Task.WhenAll(action(), action2(), action3());
            await Task.Delay(100);
            Assert.Equal(4, Counter);
        }
        private int Counter;
        private async Task SumAsync(int v, string key)
        {
            Func<Task> action = async () => await CountAsync(v);
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(action.RunUnderRaceConditionAsync(key));
            }
            await Task.WhenAll(tasks);
        }
        private async Task SumAsync2(int v, string key)
        {
            Func<Task> action = async () => await CountAsync(v);
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(RaceCondition.RunAsync(action, key));
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
