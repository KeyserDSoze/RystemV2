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
        [NoDocumentAttribute]
        public string Ale { get; set; }
        [PartitionKey]
        public string Ale1 { get; set; }
        [RowKey]
        public string Ale2 { get; set; }
        public string Ale3 { get; set; }
        public MiniSample Mini { get; set; }
        [Timestamp]
        public DateTime Timestamp { get; set; }
        public RystemDocumentServiceProvider ConfigureDocument()
        {
            return RystemDocumentServiceProvider
                .WithAzure()
                .WithTableStorage()
                .AndWithAzure(Installation.Inst00)
                .WithBlobStorage();
        }
        public static async Task Run(Installation installation)
        {
            await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd", Ale3 = "dddddddddddddddddd", Mini = new MiniSample { X = 3, Ale3 = "" }, Timestamp = DateTime.UtcNow }.UpdateAsync(installation).NoContext();
            await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd3", Ale3 = "dddddddddddddddddd", Mini = new MiniSample { X = 4, Ale3 = "ddd" }, Timestamp = DateTime.UtcNow }.UpdateAsync(installation).NoContext();
            var elements = (await new Sample().ToListAsync(x => x.Ale1 == "dddd", installation).NoContext()).ToList();
            Assert.Equal(2, elements.Count);
            foreach (var x in elements)
                if (await x.ExistsAsync(installation).NoContext())
                    await x.DeleteAsync(installation).NoContext();
            elements = (await new Sample().ToListAsync(x => x.Ale1 == "dddd", installation).NoContext()).ToList();
            Assert.Empty(elements);
        }
    }
    public class MiniSample
    {
        public string Ale3 { get; set; }
        public double X { get; set; }
    }
}
