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
    }
}
