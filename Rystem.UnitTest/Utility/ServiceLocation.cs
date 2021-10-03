using Microsoft.Extensions.DependencyInjection;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class ServiceLocation
    {
        static ServiceLocation()
        {
            ServiceLocator
                .Create()
                .AddSingleton(new FalseNueve() { Al = "A", Ol = "B" })
                .FinalizeWithoutDependencyInjection();
        }

        [Fact]
        public async Task IsOk()
        {
            var falseNueve = new Service<FalseNueve>();
            await Task.Delay(10);
            Assert.Equal("B", falseNueve.Value.Ol);
            Assert.Equal("A", falseNueve.Value.Al);
            string random = falseNueve.Value.Random;
            ServiceLocatorAtRuntime
                .PrepareToAddNewService()
                .AddTransient<Zen>();
            ServiceLocatorAtRuntime.Rebuild();
            falseNueve = new Service<FalseNueve>();
            Assert.Equal(random, falseNueve.Value.Random);
            var zen = new Service<Zen>();
            Assert.Null(zen.Value.Olaf);
        }
        private class FalseNueve
        {
            public string Al { get; set; }
            public string Ol { get; set; }
            public string Random { get; set; } = Guid.NewGuid().ToString();
        }
        private class Zen
        {
            public string Olaf { get; set; }
        }
    }
}
