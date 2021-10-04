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

namespace Rystem.UnitTest.Business.Document
{
    public class DocumentTest
    {
        static DocumentTest()
        {
            AzureConst.Load()
                .UseDocumentOn<Sample>()
                .WithAzure()
                .WithTableStorage()
                .WithPrimaryKey(x => x.PrimaryKey)
                .WithSecondaryKey(x => x.SecondaryKey)
                .WithTimestamp(x => x.Timestamp)
                .AndWithAzure(Installation.Inst00)
                .WithBlobStorage(new Azure.Integration.Storage.BlobStorageConfiguration
                {
                    Name = "coldwea"
                })
                .WithPrimaryKey(x => x.PrimaryKey)
                .WithSecondaryKey(x => x.SecondaryKey)
                .WithTimestamp(x => x.Timestamp)
                .AndWithAzure(Installation.Inst01)
                .WithCosmosNoSql(new Azure.Integration.Cosmos.CosmosConfiguration(DatabaseName: "Colon"))
                .WithPrimaryKey(x => x.PrimaryKey)
                .WithSecondaryKey(x => x.SecondaryKey)
                .WithTimestamp(x => x.Timestamp)
                .Configure()
                .FinalizeWithoutDependencyInjection();
        }
        [Fact]
        public async Task TableStorage()
        {
            await Sample.Run(Installation.Default).NoContext();
        }
        [Fact]
        public async Task BlobStorage()
        {
            await Sample.Run(Installation.Inst00).NoContext();
        }
        [Fact]
        public async Task Cosmos()
        {
            await Sample.Run(Installation.Inst01).NoContext();
        }
        [Fact]
        public async Task Runtime()
        {
            List<Task> tasks = new();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(new MyFirstDocument().FirstOrDefaultAsync());
            }
            await Task.WhenAll(tasks);
            await Task.WhenAll(new MyGreatDocument().FirstOrDefaultAsync(), new MyGuruDocument().FirstOrDefaultAsync(), new MyDocument().FirstOrDefaultAsync()).NoContext();
            var t = await new MyDocument().FirstOrDefaultAsync().NoContext();
            Assert.Null(t);
#warning to manage the abstracions, probably there's a bug on c#
            //string name = new MyRealDocument().GetName();
            //Assert.Equal(nameof(MyRealDocument), name);
            //name = new MyRealDocument2().GetName();
            //Assert.Equal(nameof(MyRealDocument2), name);
        }
        public class MyRealDocument : MyDocument
        {

        }
        public class MyRealDocument2 : MyDocument
        {

        }
        public class MyFirstDocument : IConfigurableDocument
        {
            public string PrimaryKey { get; set; }
            public string SecondaryKey { get; set; }
            public string Ale3 { get; set; }
            public MiniSample Mini { get; set; }
            public DateTime Timestamp { get; set; }
            public RystemDocumentServiceProvider Configure(string callerName)
            {
                return this.StartConfiguration()
                    .WithAzure()
                    .WithTableStorage(new Azure.Integration.Storage.TableStorageConfiguration(callerName))
                    .WithPrimaryKey(x => x.PrimaryKey)
                    .WithSecondaryKey(x => x.SecondaryKey)
                    .WithTimestamp(x => x.Timestamp)
                    .ConfigureAfterBuild();
            }
        }
        public class MyDocument : IConfigurableDocument
        {
            public string PrimaryKey { get; set; }
            public string SecondaryKey { get; set; }
            public string Ale3 { get; set; }
            public MiniSample Mini { get; set; }
            public DateTime Timestamp { get; set; }
            public RystemDocumentServiceProvider Configure(string callerName)
            {
                return this.StartConfiguration()
                    .WithAzure()
                    .WithTableStorage(new Azure.Integration.Storage.TableStorageConfiguration(callerName))
                    .WithPrimaryKey(x => x.PrimaryKey)
                    .WithSecondaryKey(x => x.SecondaryKey)
                    .WithTimestamp(x => x.Timestamp)
                    .ConfigureAfterBuild();
            }
        }
        public class MyGreatDocument : IConfigurableDocument
        {
            public string PrimaryKey { get; set; }
            public string SecondaryKey { get; set; }
            public string Ale3 { get; set; }
            public MiniSample Mini { get; set; }
            public DateTime Timestamp { get; set; }
            public RystemDocumentServiceProvider Configure(string callerName)
            {
                return this.StartConfiguration()
                    .WithAzure()
                    .WithTableStorage(new Azure.Integration.Storage.TableStorageConfiguration(callerName))
                    .WithPrimaryKey(x => x.PrimaryKey)
                    .WithSecondaryKey(x => x.SecondaryKey)
                    .WithTimestamp(x => x.Timestamp)
                    .ConfigureAfterBuild();
            }
        }
        public class MyGuruDocument : IConfigurableDocument
        {
            public string PrimaryKey { get; set; }
            public string SecondaryKey { get; set; }
            public string Ale3 { get; set; }
            public MiniSample Mini { get; set; }
            public DateTime Timestamp { get; set; }
            public RystemDocumentServiceProvider Configure(string callerName)
            {
                return this.StartConfiguration()
                    .WithAzure()
                    .WithTableStorage(new Azure.Integration.Storage.TableStorageConfiguration(callerName))
                    .WithPrimaryKey(x => x.PrimaryKey)
                    .WithSecondaryKey(x => x.SecondaryKey)
                    .WithTimestamp(x => x.Timestamp)
                    .ConfigureAfterBuild();
            }
        }
    }
}