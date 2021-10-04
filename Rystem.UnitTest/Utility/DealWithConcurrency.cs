using Rystem.Concurrency;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
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
        [Fact]
        public async Task RunTheConcurrentList()
        {
            int max = 20;
            ConcurrentBag<string> bags = new();
            for (int i = 0; i < max; i++)
                ThreadPool.QueueUserWorkItem(async (x) =>
                {
                    await Task.WhenAll(GoAsync(), ReadAsync());

                    Task GoAsync()
                    {
                        for (int j = 0; j < max; j++)
                            bags.Add($"{i}-{j}");
                        return Task.CompletedTask;
                    }
                    async Task ReadAsync()
                    {
                        for (int j = 0; j < max; j++)
                        {
                            await Task.Delay(1);
                            foreach (var c in bags)
                            {
                                _ = c.ToLower();
                            }
                        }
                    }
                });
            await Task.Delay(4000);
            Assert.Equal(max * max, bags.Count);
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
