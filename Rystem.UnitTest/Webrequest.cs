using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class Webrequest
    {
        [Fact]
        public async Task TryToMakeAGet()
        {
            string response = await new Uri("https://www.google.com")
                .CreateHttpRequest()
                .Build()
                .InvokeAsync();
            Assert.NotEqual(0, response.Length);
        }
        [Fact]
        public async Task TryToParseJson()
        {
            var response = await new Uri("https://jsonplaceholder.typicode.com/todos/1")
                .CreateHttpRequest()
                .Build()
                .InvokeAsync<Rootobject>();
            Assert.Equal(1, response.id);
        }
        public class Rootobject
        {
            public int userId { get; set; }
            public int id { get; set; }
            public string title { get; set; }
            public bool completed { get; set; }
        }
    }
}
