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
    internal sealed class DealWithLockBusiness
    {
        public async Task RunASingleTimeOnTwoLocks(Installation installation = default)
        {
            int n = 2;
            async Task action() => await SumAsync(n, "3ad2", 1, installation);
            async Task action2() => await SumAsync(n, "3ad44", 2, installation);
            async Task action3() => await SumAsync(n, "3ad21", 3, installation);
            await Task.WhenAll(action(), action2(), action3());
            Assert.Equal(MaxiCounter, Counter + Counter1 + Counter2 + Error * n);
        }
        public async Task RunASingleTimeOnTwoLocks2(Installation installation)
        {
            int n = 2;
            async Task action() => await SumAsync2(n, "3ad", 1, installation);
            async Task action2() => await SumAsync2(n, "3ad44", 2, installation);
            async Task action3() => await SumAsync2(n, "3ad2", 3, installation);
            await Task.WhenAll(action(), action2(), action3());
            Assert.Equal(120, Counter + Counter1 + Counter2 + Error * n);
        }
        private int Counter;
        private int Counter1;
        private int Counter2;
        private int Error;
        private int MaxiCounter;
        private async Task SumAsync(int v, string key, int c, Installation installation)
        {
            Func<Task> action = async () => await CountAsync(v, c);
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 20; i++)
            {
                MaxiCounter += v;
                tasks.Add(Execute());
            }
            await Task.WhenAll(tasks);
            async Task Execute()
            {
                var x = await new DealingKey() { Key = key }.LockAsync(action, installation).NoContext();
                if (x.InException)
                    Error++;
            };
        }
        private async Task SumAsync2(int v, string key, int c, Installation installation)
        {
            Func<Task> action = async () => await CountAsync(v, c);
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(Execute());
            }
            await Task.WhenAll(tasks);
            async Task Execute()
            {
                var x = await new DealingKey() { Key = key }.LockAsync(action, installation).NoContext();
                if (x.InException)
                    Error++;
            };
        }
        private async Task CountAsync(int v, int c)
        {
            await Task.Delay(15);
            if (c == 1)
                Counter += v;
            else if (c == 2)
                Counter1 += v;
            else
                Counter2 += v;
        }
    }
}
