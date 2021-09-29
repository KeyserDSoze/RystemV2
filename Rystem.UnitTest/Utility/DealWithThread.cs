using Microsoft.Extensions.DependencyInjection;
using Rystem.Background;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class DealWithThread
    {
        static DealWithThread()
        {
            AzureConst.Load()
              .AddBackgroundJob<MyFirstBackgroundWork>(x =>
               {
                   x.Cron = "* * * * * *";
                   x.RunImmediately = true;
                   x.Key = "The best of the keys";
               })
               .AddSingleton<MyDiTest>()
               .FinalizeWithoutDependencyInjection();
        }
        [Fact]
        public async Task RunInBackground()
        {
            Action action = async () => await CountAsync(2);
            await action.RunInBackgroundAsync("3", () => 300).NoContext();
            await Task.Delay(1400);
            await action.StopRunningInBackgroundAsync("3").NoContext();
            Assert.Equal(8, Counter);
        }
        [Fact]
        public async Task RunInBackground2()
        {
            await BackgroundJob.RunAsync(async () => await CountAsync(2), nextRunningTime: () => 300, runImmediately: true).NoContext();
            await Task.Delay(1100);
            await BackgroundJob.StopAsync().NoContext();
            Assert.Equal(8, Counter);
        }
        [Fact]
        public async Task RunInIServiceCollection()
        {
            await Task.Delay(2000);
            await BackgroundJob.StopAsync().NoContext();
            var myDi = ServiceLocator.GetService<MyDiTest>();
            Assert.Equal(6, myDi.Counter);
        }
        private class MyDiTest
        {
            public int Counter { get; set; }
        }
        private class MyFirstBackgroundWork : IBackgroundJob
        {
            private readonly MyDiTest MyDiTest;
            public MyFirstBackgroundWork(MyDiTest myDiTest)
                => MyDiTest = myDiTest;
            public async Task ActionToDoAsync()
            {
                await Task.Delay(0).NoContext();
                MyDiTest.Counter += 2;
            }
            public Task OnException(Exception exception)
            {
                return Task.CompletedTask;
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
