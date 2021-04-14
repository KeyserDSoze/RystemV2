using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class Json
    {
        [Fact]
        public async Task IsOk()
        {
            var falseNueve = new FalseNueve()
            {
                Al = "a",
                Ol = "b"
            };
            var json = falseNueve.ToJson();
            var falseNueve2 = json.FromJson<FalseNueve>();
            Assert.Equal("a", falseNueve2.Al);
            Assert.Equal("b", falseNueve2.Ol);
        }
        private class FalseNueve
        {
            public string Al { get; set; }
            public string Ol { get; set; }
        }
    }
}
