using Rystem.Azure.Installation;
using Rystem.Azure.Integration;
using Rystem.Business;
using Rystem.Business.Document;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RystemV2
{
    class Program
    {
        private sealed class AppSetting
        {
            public StorageSetting Storage { get; init; }
            public sealed record StorageSetting(string Name, string Key);
        }
#warning Allow the loading through a json or appsettings directly
        static Program()
        {
            var settings = File.ReadAllText("appsettings.json").FromJson<AppSetting>();
            RystemInstaller
                .WithAzure()
                .AddStorage(new Rystem.Azure.Integration.Storage.StorageOptions(settings.Storage.Name, settings.Storage.Key))
                .Build();
        }
        static async Task Main(string[] args)
        {
            //(new Sample() as IDocument).Build();
            //await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd", Ale3 = "dddddddddddddddddd", Timestamp = DateTime.UtcNow }.UpdateAsync(Installation.Inst00).NoContext();
            //await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd3", Ale3 = "dddddddddddddddddd", Timestamp = DateTime.UtcNow }.UpdateAsync(Installation.Inst00).NoContext();
            //var x = (await new Sample().ToListAsync(x => x.Ale1 == "dddd", Installation.Inst00).NoContext()).ToList();
            await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd", Ale3 = "dddddddddddddddddd", Timestamp = DateTime.UtcNow }.SendAsync().NoContext();
            await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd3", Ale3 = "dddddddddddddddddd", Timestamp = DateTime.UtcNow }.SendAsync().NoContext();
            var x = (await new Sample().ReadAsync().NoContext()).ToList();
        }
        public class Sample : IDocument, IQueue
        {
            [NoDocumentAttribute]
            public string Ale { get; set; }
            [PartitionKeyAttribute]
            public string Ale1 { get; set; }
            [RowKeyAttribute]
            public string Ale2 { get; set; }
            public string Ale3 { get; set; }
            [TimestampAttribute]
            public DateTime Timestamp { get; set; }
            public RystemDocumentServiceProvider ConfigureDocument()
            {
                return RystemDocumentServiceProvider
                    .WithAzure()
                    .WithTableStorage()
                    .AndWithAzure(Installation.Inst00)
                    .WithBlobStorage();
            }

            public RystemQueueServiceProvider ConfigureQueue()
            {
                return RystemQueueServiceProvider
                    .WithAzure()
                    .WithQueueStorage();
            }
        }
    }
}
