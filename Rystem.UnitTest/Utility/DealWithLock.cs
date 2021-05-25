using Rystem.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class DealWithLock
    {
        [Fact]
        public async Task RunASingleTimeOnTwoLocks()
        {
            int n = 2;
            async Task action() => await SumAsync(n, "3ad2", 1);
            async Task action2() => await SumAsync(n, "3ad44", 2);
            async Task action3() => await SumAsync(n, "3ad21", 3);
            await Task.WhenAll(action(), action2(), action3());
            Assert.Equal(MaxiCounter, Counter + Counter1 + Counter2 + Error * n);
        }
        [Fact]
        public async Task RunASingleTimeOnTwoLocks2()
        {
            int n = 2;
            async Task action() => await SumAsync2(n, "3ad", 1);
            async Task action2() => await SumAsync2(n, "3ad44", 2);
            async Task action3() => await SumAsync2(n, "3ad2", 3);
            await Task.WhenAll(action(), action2(), action3());
            Assert.Equal(120, Counter + Counter1 + Counter2 + Error * n);
        }
        private int Counter;
        private int Counter1;
        private int Counter2;
        private int Error;
        private int MaxiCounter;
        private async Task SumAsync(int v, string key, int c)
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
                var x = await action.LockAsync(key);
                if (x.InException)
                    Error++;
            };
        }
        private async Task SumAsync2(int v, string key, int c)
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
                var x = await Lock.RunAsync(action, key);
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
