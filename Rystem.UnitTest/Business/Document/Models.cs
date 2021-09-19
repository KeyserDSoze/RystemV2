using Rystem.Azure.Integration;
using Rystem.Business;
using Rystem.Business.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Business.Document
{
    public class Sample : IDocument
    {
        [NoDocument]
        public string Ale { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public string Ale3 { get; set; }
        public MiniSample Mini { get; set; }
        public DateTime Timestamp { get; set; }
        public RystemDocumentServiceProvider ConfigureDocument()
        {
            return RystemDocumentServiceProvider
                .WithAzure()
                .WithTableStorage()
                .AndWithAzure(Installation.Inst00)
                .WithBlobStorage(new Azure.Integration.Storage.BlobStorageConfiguration
                {
                    Name = "coldwea"
                })
                .AndWithAzure(Installation.Inst01)
                .WithCosmosNoSql(new Azure.Integration.Cosmos.CosmosConfiguration(DatabaseName: "Colon"));
        }
        public static async Task Run(Installation installation)
        {
            var elements = (await new Sample().ListAsync(installation: installation).NoContext()).ToList();
            await elements.DeleteBatchAsync(installation).NoContext();
            await CreateNewSample(1).UpdateAsync(installation).NoContext();
            await CreateNewSample(1).UpdateAsync(installation).NoContext();
            elements = (await new Sample().ListAsync(x => x.PrimaryKey == "1", installation).NoContext()).ToList();
            Assert.Equal(2, elements.Count);
            foreach (var x in elements)
                if (await x.ExistsAsync(installation).NoContext())
                    await x.DeleteAsync(installation).NoContext();
            elements = (await new Sample().ListAsync(x => x.PrimaryKey == 1.ToString(), installation).NoContext()).ToList();
            Assert.Empty(elements);
            List<Sample> samples = new();
            for (int i = 0; i < 10; i++)
                samples.Add(CreateNewSample(i));
            await samples.UpdateBatchAsync(installation).NoContext();
            elements = (await new Sample().ListAsync(x => x.PrimaryKey == 1.ToString(), installation).NoContext()).ToList();
            Assert.Single(elements);
            elements = (await new Sample().ListAsync(installation: installation).NoContext()).ToList();
            Assert.Equal(10, elements.Count);
            foreach (var x in elements)
                if (await x.ExistsAsync(installation).NoContext())
                    await x.DeleteAsync(installation).NoContext();
            elements = (await new Sample().ListAsync(x => x.PrimaryKey == 1.ToString(), installation).NoContext()).ToList();
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