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
        [Fact]
        public async Task Runtime()
        {
            List<Task> tasks = new();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(new MyFirstQueue().ReadAsync());
            }
            await Task.WhenAll(tasks);
            await Task.WhenAll(new MyGreatQueue().ReadAsync(), new MyGuruQueue().ReadAsync(), new MyQueue().ReadAsync()).NoContext();
            var t = await new MyQueue().ReadAsync().NoContext();
            Assert.Empty(t);
#warning to manage the abstracions
            //string name = new MyRealDocument().GetName();
            //Assert.Equal(nameof(MyRealDocument), name);
            //name = new MyRealDocument2().GetName();
            //Assert.Equal(nameof(MyRealDocument2), name);
        }
        public class MyRealDocument : MyQueue
        {

        }
        public class MyRealDocument2 : MyQueue
        {

        }
        public class MyFirstQueue : IConfigurableQueue
        {
            public string PrimaryKey { get; set; }
            public string SecondaryKey { get; set; }
            public string Ale3 { get; set; }
            public MiniSample Mini { get; set; }
            public DateTime Timestamp { get; set; }
            public RystemQueueServiceProvider Configure(string callerName)
                => this.StartConfiguration()
                    .WithAzure()
                    .WithQueueStorage()
                    .ConfigureAfterBuild();
        }
        public class MyQueue : IConfigurableQueue
        {
            public string PrimaryKey { get; set; }
            public string SecondaryKey { get; set; }
            public string Ale3 { get; set; }
            public MiniSample Mini { get; set; }
            public DateTime Timestamp { get; set; }
            public RystemQueueServiceProvider Configure(string callerName)
            {
                return this.StartConfiguration()
                    .WithAzure()
                    .WithEventHub(new Azure.Integration.Message.EventHubConfiguration() { Name = "Xyz" })
                    .ConfigureAfterBuild();
            }
        }
        public class MyGreatQueue : IConfigurableQueue
        {
            public string PrimaryKey { get; set; }
            public string SecondaryKey { get; set; }
            public string Ale3 { get; set; }
            public MiniSample Mini { get; set; }
            public DateTime Timestamp { get; set; }
            public RystemQueueServiceProvider Configure(string callerName)
            {
                return this.StartConfiguration()
                    .WithAzure()
                    .WithEventHub(new Azure.Integration.Message.EventHubConfiguration() { Name = "Xyz" })
                    .ConfigureAfterBuild();
            }
        }
        public class MyGuruQueue : IConfigurableQueue
        {
            public string PrimaryKey { get; set; }
            public string SecondaryKey { get; set; }
            public string Ale3 { get; set; }
            public MiniSample Mini { get; set; }
            public DateTime Timestamp { get; set; }
            public RystemQueueServiceProvider Configure(string callerName)
            {
                return this.StartConfiguration()
                    .WithAzure()
                    .WithEventHub(new Azure.Integration.Message.EventHubConfiguration() { Name = "Xyz" })
                    .ConfigureAfterBuild();
            }
        }
    }
}