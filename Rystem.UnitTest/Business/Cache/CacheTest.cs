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

namespace Rystem.UnitTest.Business.Cache
{
    public class CacheTest
    {
        static CacheTest()
        {
            new TestHost(AzureConst.Load()
                .UseCacheWithKey<SampleKey, Sample>()
                .WithAzure(new CacheConfiguration(TimeSpan.FromMinutes(5)), Installation.Default)
                .WithTableStorage()
                .AndWithAzure(default, Installation.Inst00)
                .WithBlobStorage()
                .AndWithAzure(new CacheConfiguration(TimeSpan.FromMinutes(5)), Installation.Inst01)
                .WithRedisCache()
                .AndMemory(new CacheConfiguration(TimeSpan.FromSeconds(1)))
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
        public async Task RedisCache()
        {
            await Sample.Run(Installation.Inst01).NoContext();
        }
    }
}