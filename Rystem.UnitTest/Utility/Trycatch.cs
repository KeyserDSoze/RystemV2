using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Polly;
using Polly.Registry;

namespace Rystem.UnitTest
{
    public class Trycatch
    {
        private static string A = "";
        static Trycatch()
        {
            new ServiceCollection()
                .AddRystem()
                .AddRetryPolicy<Exception>(x =>
                {
                    A = x.Message;
                    return true;
                })
                .RetryAsync(3)
                .AsAsyncPolicy<Exception>()
                .Configure();
        }
        private static async Task<Exception> ErrorAsync()
        {
            await Task.Delay(10);
            if (A == "")
                throw new ArgumentException("aaaaa");
            else
                return default;
        }
        [Fact]
        public async Task TryToCatch()
        {
            var registry = ServiceLocator.GetService<PolicyRegistry>();
            await registry.Get<IAsyncPolicy<Exception>>("")
                .ExecuteAsync(ErrorAsync).NoContext();
            Assert.Equal("aaaaa", A);
        }
        [Fact]
        public async Task TryExtension()
        {
            var response = await Try.ExecuteAsync(async () =>
            {
                await Task.Delay(0);
                return "a";
            }).NoContext();
            Assert.Equal("a", response.Result);
            Assert.True(response.Executed);
            var watch = Stopwatch.Start();
            response = await Try.ExecuteAsync<string>(async () =>
            {
                await Task.Delay(0);
                throw new Exception("calc");
            }, 10, TimeSpan.FromMilliseconds(10)).NoContext();
            var r = watch.Stop();
            Assert.Equal(default, response.Result);
            Assert.False(response.Executed);
            Assert.Equal(10, response.Exceptions.Count);
            Assert.True(r.Span > TimeSpan.FromMilliseconds(10 * 10));
        }
    }
}