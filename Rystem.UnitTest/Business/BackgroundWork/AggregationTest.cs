using Rystem.Background;
using Rystem.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Business.BackgroundWork
{
    public class AggregationTest
    {
        private class Sequencer : IAggregation<FalseNueve>
        {
            public RystemAggregationServiceProvider ConfigureSequence()
            {
                return RystemAggregationServiceProvider.Configure(Installation.Default)
                    .WithFirstInFirstOut(new SequenceProperty<FalseNueve>("FalseNueve", 500, TimeSpan.FromSeconds(2), Evaluate))
                    .AndConfigure(Installation.Inst00)
                    .WithLastInFirstOut(new SequenceProperty<FalseNueve>("FalseNueve", 500, TimeSpan.FromSeconds(2), Evaluate));
            }
            public int Counter;
            private async Task Evaluate(IEnumerable<FalseNueve> falseNueves)
            {
                foreach (var _ in falseNueves)
                {
                    Counter++;
                }
                await Task.Delay(0);
            }
        }

        [Fact]
        public async Task SequenceFifo()
        {
            await QueueSomething(Installation.Default).NoContext();
        }
        [Fact]
        public async Task SequenceLifo()
        {
            await QueueSomething(Installation.Inst00).NoContext();
        }
        private async Task QueueSomething(Installation installation)
        {
            var sequencer = new Sequencer();
            for (int i = 0; i < 510; i++)
            {
                var falseNueve = new FalseNueve()
                {
                    Al = "a",
                    Ol = "b"
                };
                sequencer.Add(falseNueve, installation);
                if (i == 499)
                    await Task.Delay(100);
            }
            await Task.Delay(700);
            Assert.Equal(500, sequencer.Counter);
            sequencer.Flush(installation);
            await Task.Delay(700);
            Assert.Equal(510, sequencer.Counter);
        }
        private interface IFalseNueve
        {
            string Al { get; set; }
            string Ol { get; set; }
        }
        private class FalseNueve : IFalseNueve
        {
            public string Al { get; set; }
            public string Ol { get; set; }
        }
    }
}
