using Microsoft.Extensions.DependencyInjection;
using Rystem.Background;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class DealWithThread
    {
        [Fact]
        public async Task RunInBackground()
        {
            Action action = async () => await CountAsync(2);
            action.RunInBackground("3", () => 300);
            await Task.Delay(1400);
            action.StopRunningInBackground("3");
            Assert.Equal(8, Counter);
        }
        [Fact]
        public async Task RunInBackground2()
        {
            BackgroundWork.Run(async () => await CountAsync(2), nextRunningTime: () => 300, runImmediately: true);
            await Task.Delay(1100);
            BackgroundWork.Stop();
            Assert.Equal(8, Counter);
        }
        [Fact]
        public async Task RunInIServiceCollection()
        {
            new ServiceCollection()
                .AddBackgroundWork<MyFirstBackgroundWork>(x =>
                {
                    x.Cron = "* * * * * *";
                    x.RunImmediately = true;
                    x.Key = "The best of the keys";
                })
                .AddSingleton<MyDiTest>()
                .WithRystem();
            await Task.Delay(2600);
            BackgroundWork.Stop();
            var myDi = RystemManager.GetService<MyDiTest>();
            Assert.Equal(6, myDi.Counter);
        }
        private class MyDiTest
        {
            public int Counter { get; set; }
        }
        private class MyFirstBackgroundWork : IBackgroundWork
        {
            private readonly MyDiTest MyDiTest;
            public MyFirstBackgroundWork(MyDiTest myDiTest)
                => MyDiTest = myDiTest;
            public async Task ActionToDoAsync()
            {
                await Task.Delay(0).NoContext();
                MyDiTest.Counter += 2;
            }
        }
        private int Counter;
        private async Task CountAsync(int v)
        {
            await Task.Delay(0).NoContext();
            Counter += v;
        }
    }
}
