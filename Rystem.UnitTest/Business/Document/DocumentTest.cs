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
            new TestHost(AzureConst.Load()
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
                .Configure())
                .WithRystem();


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
    }
}