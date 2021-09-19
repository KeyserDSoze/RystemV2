using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
using System.Threading;
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
            services.UseDataOn<Ola>()
                .WithAzure()
                .WithBlockBlob()
                .WithName(x => x.Name)
                .Configure();
            new Host(services).WithRystem();
        }
        public class Host : IHost
        {
            public IServiceProvider Services { get; }
            public Host(IServiceCollection services)
                => Services = services.BuildServiceProvider();

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public Task StartAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task StopAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }

        public class Ola
        {
            public string Name { get; set; }
        }
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
            var data = ServiceLocator.GetService<IDataManager<Ola>>();
            await data.WriteAsync("", new byte[1], null).NoContext();
        }
        public class Sample : IQueue
        {
            [NoDocument]
            public string Ale { get; set; }
            public string Ale1 { get; set; }
            public string Ale2 { get; set; }
            public string Ale3 { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
