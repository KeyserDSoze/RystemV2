using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class Trycatch
    {
        [Fact]
        public async Task TryToCatch()
        {
            string a = string.Empty;
            await Try.Execute(async () =>
            {
                await Task.Delay(10);
                throw new ArgumentException("aaaaa");
            })
                .Catch<ArgumentException>(async x =>
                {
                    await Task.Delay(10);
                    a = x.ToString();
                })
                .InvokeAsync();
            Assert.NotEqual(string.Empty, a);
        }
        [Fact]
        public async Task TryToNotCatch()
        {
            string a = string.Empty;
            await Try.Execute(async () =>
            {
                await Task.Delay(10);
                throw new Exception("aaaaa");
            })
                .Catch<ArgumentException>(async x =>
                {
                    await Task.Delay(10);
                    a = x.ToString();
                })
                .InvokeAsync();
            Assert.Equal(string.Empty, a);
        }
    }
}
