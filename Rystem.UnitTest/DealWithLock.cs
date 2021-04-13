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
            async Task action() => await SumAsync(2, "3ad");
            async Task action2() => await SumAsync(2, "3ad44");
            async Task action3() => await SumAsync(2, "3ad2");
            await Task.WhenAll(action(), action2(), action3());
            Assert.Equal(120, Counter + Error * 2);
        }
        private int Counter;
        private int Error;
        private async Task SumAsync(int v, string key)
        {
            Func<Task> action = async () => await CountAsync(v);
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 20; i++)
            {
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
        private static readonly object Semaphore = new object();
        private async Task CountAsync(int v)
        {
            await Task.Delay(15);
            //I am using lock here because sometimes a multiple sum operation occurs in a wrong operation (due to memory concurrency operations).
            lock (Semaphore)
                Counter += v;
        }
    }
}
