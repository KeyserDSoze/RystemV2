using Microsoft.Extensions.DependencyInjection;
using Rystem;
using Rystem.Azure;
using Rystem.Azure.Integration;
using Rystem.Azure.Integration.Storage;
using Rystem.Business;
using Rystem.Business.Data;
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
            var services = new ServiceCollection();
            services
                .AddAzureService()
                .AddStorage(new StorageAccount(settings.Storage.Name, settings.Storage.Key))
                .EndConfiguration();
            services.UseCacheWithKey<KeyForge, Ola>()
                .AndMemory()
                .AndWithAzure()
                .WithBlobStorage()
                .Configure();
            services.UseData<Ola>()
                .WithAzure()
                .WithBlockBlob()
                .Configure();
        }
        public class Ola { }
        public class KeyForge : ICacheKey<Ola>
        {
            public Task<Ola> FetchAsync()
            {
                throw new NotImplementedException();
            }
        }
        static async Task Main(string[] args)
        {
            //(new Sample() as IDocument).Build();
            //await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd", Ale3 = "dddddddddddddddddd", Timestamp = DateTime.UtcNow }.UpdateAsync(Installation.Inst00).NoContext();
            //await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd3", Ale3 = "dddddddddddddddddd", Timestamp = DateTime.UtcNow }.UpdateAsync(Installation.Inst00).NoContext();
            //var x = (await new Sample().ToListAsync(x => x.Ale1 == "dddd", Installation.Inst00).NoContext()).ToList();
            //await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd", Ale3 = "dddddddddddddddddd", Timestamp = DateTime.UtcNow }.SendAsync().NoContext();
            //await new Sample() { Ale = "ddd", Ale1 = "dddd", Ale2 = "dddddddd3", Ale3 = "dddddddddddddddddd", Timestamp = DateTime.UtcNow }.SendAsync().NoContext();
            //var x = (await new Sample().ReadAsync().NoContext()).ToList();
            var data = ServiceLocator.GetService<Data<Ola>>();
            await data.WriteAsync("", new byte[1]).NoContext();
        }
        public class Sample : IQueue
        {
            [NoDocument]
            public string Ale { get; set; }
            public string Ale1 { get; set; }
            public string Ale2 { get; set; }
            public string Ale3 { get; set; }
            public DateTime Timestamp { get; set; }
            public RystemQueueServiceProvider ConfigureQueue()
            {
                return RystemQueueServiceProvider
                    .WithAzure()
                    .WithQueueStorage();
            }
        }
    }
}
