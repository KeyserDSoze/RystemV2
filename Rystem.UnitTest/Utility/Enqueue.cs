using Rystem.Background;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class Enqueue
    {
        private const string QueueName = "FalseNueve";
        private async Task Evaluate(IEnumerable<IFalseNueve> falseNueves)
        {
            foreach (var _ in falseNueves)
            {
                Counter++;
            }
            await Task.Delay(0);
        }
        private int Counter;
        [Fact]
        public async Task QueueSomething()
        {
            Queue.Create<IFalseNueve>(new SequenceProperty<IFalseNueve>(QueueName, 500, TimeSpan.FromSeconds(2), Evaluate), QueueType.FirstInFirstOut);
            for (int i = 0; i < 510; i++)
            {
                var falseNueve = new FalseNueve()
                {
                    Al = "a",
                    Ol = "b"
                };
                falseNueve.Enqueue(QueueName);
                if (i == 499)
                    await Task.Delay(100);
            }
            await Task.Delay(700);
            Assert.Equal(500, Counter);
            Queue.Flush(QueueName, true);
            await Task.Delay(700);
            Assert.Equal(510, Counter);
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
