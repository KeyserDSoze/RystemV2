using Rystem.Azure.Installation;
using Rystem.Business;
using Rystem.Business.AzureAttribute;
using Rystem.Business.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RystemV2
{
    class Program
    {
        static Program()
        {
            RystemInstaller
                .WithAzure()
                .AddStorage(new Rystem.Azure.Integration.Storage.StorageOptions("csb10032000f4ec96f7", "/LX0M5FTEmsemK/vhSg4KGEogsD8BxkZtv+KLVBNkHcjCSgnjDXpPysprOMzjmQFu31mJ1d5cbP5gxV3oFqXdw=="))
                .Build();
        }
        static async Task Main(string[] args)
        {
            //(new Sample() as IDocument).Build();
            await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd", Ale3 = "dddddddddddddddddd", Timestamp = DateTime.UtcNow }.UpdateAsync().NoContext();
            await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd3", Ale3 = "dddddddddddddddddd", Timestamp = DateTime.UtcNow }.UpdateAsync().NoContext();
            var x = (await new Sample().ToListAsync().NoContext()).ToList();
        }
        public class Sample : IDocument
        {
            [NoDocumentProperty]
            public string Ale { get; set; }
            [PartitionKeyProperty]
            public string Ale1 { get; set; }
            [RowKeyProperty]
            public string Ale2 { get; set; }
            public string Ale3 { get; set; }
            [TimestampProperty]
            public DateTime Timestamp { get; set; }
            public RystemDocumentServiceProvider ConfigureDocument()
            {
                return RystemDocumentServiceProvider
                    .WithAzure()
                    .WithTableStorage()
                    .AndWithAzure(Installation.Inst00)
                    .WithTableStorage();
            }
        }
    }
}
