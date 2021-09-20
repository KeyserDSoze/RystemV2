using Rystem.Azure.Integration;
using Rystem.Business;
using Rystem.Business.Document;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Business.Queue
{
    public class QueueTest
    {
        static QueueTest()
        {
            AzureConst.Load()
                .UseQueueOn<Sample>()
                .WithAzure()
                .WithQueueStorage()
                .AndWithAzure(Installation.Inst00)
                .WithEventHub(new Azure.Integration.Message.EventHubConfiguration() { Name = "Xyz" })
                .AndWithAzure(Installation.Inst01)
                .WithServiceBus(new Azure.Integration.Message.ServiceBusConfiguration() { Name = "Xyz" })
                .Configure()
                .FinalizeWithoutDependencyInjection();
        }
        [Fact]
        public async Task QueueStorage()
        {
            await Sample.Run(Installation.Default).NoContext();
        }
        [Fact]
        public async Task EventHub()
        {
            await Sample.Run(Installation.Inst00).NoContext();
        }
        [Fact]
        public async Task ServiceBus()
        {
            await Sample.Run(Installation.Inst01).NoContext();
        }
    }
}