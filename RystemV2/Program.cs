using Rystem.BackgroundWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RystemV2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Sequence.Create<IFalseNueve>(500, TimeSpan.FromSeconds(2), Evaluate, QueueName, QueueType.FirstInFirstOut);
            IFalseNueve falseNueve = new FalseNueve()
            {
                Al = "a",
                Ol = "b"
            };
            falseNueve.Enqueue(QueueName);
            while (true)
            {
                await Task.Delay(1000);
            }
        }
        private const string QueueName = "FalseNueve";
        private static int Counter;
        private static async Task Evaluate(IEnumerable<IFalseNueve> falseNueves)
        {
            foreach (var x in falseNueves)
            {
                Console.WriteLine(x.Al);
            }
            await Task.Delay(0);
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
