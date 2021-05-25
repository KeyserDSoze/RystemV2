using Rystem.Azure.Integration;
using Rystem.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Business.Queue
{
    public class Sample : IQueue
    {
        public string Ale { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public string Ale3 { get; set; }
        public MiniSample Mini { get; set; }
        public DateTime Timestamp { get; set; }

        public RystemQueueServiceProvider ConfigureQueue()
        {
            return RystemQueueServiceProvider
                .WithAzure()
                .WithQueueStorage()
                .AndWithAzure(Installation.Inst00)
                .WithEventHub(new Azure.Integration.Message.EventHubConfiguration() { Name = "Xyz" })
                .AndWithAzure(Installation.Inst01)
                .WithServiceBus(new Azure.Integration.Message.ServiceBusConfiguration() { Name = "Xyz" });
        }
        public static async Task Run(Installation installation)
        {
            var elements = (await new Sample().ReadAsync(installation: installation).NoContext()).ToList();
            await Task.Delay(1000);
            elements = (await new Sample().ReadAsync(installation: installation).NoContext()).ToList();
            await CreateNewSample(1).SendAsync(installation: installation).NoContext();
            await CreateNewSample(1).SendAsync(installation: installation).NoContext();
            await Task.Delay(2000);
            elements = (await new Sample().ReadAsync(installation: installation).NoContext()).ToList();
            if (installation == Installation.Inst00)
                Assert.True(elements.Count >= 2);
            else
                Assert.Equal(2, elements.Count);
            elements = (await new Sample().ReadAsync(installation: installation).NoContext()).ToList();
            Assert.Empty(elements);
        }
        private static Sample CreateNewSample(int x)
            => new() { Ale = x.ToString(), PrimaryKey = x.ToString(), SecondaryKey = null, Ale3 = x.ToString(), Timestamp = DateTime.UtcNow, Mini = new MiniSample { X = x, Ale3 = x.ToString() } };
    }
    public class MiniSample
    {
        public string Ale3 { get; set; }
        public double X { get; set; }
    }
}