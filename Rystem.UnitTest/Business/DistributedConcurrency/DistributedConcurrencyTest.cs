using System;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Business.DistributedConcurrency
{
    public class DistributedConcurrencyTest
    {
        static DistributedConcurrencyTest()
        {
            AzureConst.Load()
                .UseDistributedKey<DealingKey>()
                .WithAzure()
                .WithBlobStorage()
                .AndWithAzure(Installation.Inst00)
                .WithRedisCache()
                .Configure()
                .FinalizeWithoutDependencyInjection();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoLocksRedisCache()
        {
            await new DealWithLockBusiness().RunASingleTimeOnTwoLocks(Rystem.Installation.Inst00).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoLocksBlobStorage()
        {
            await new DealWithLockBusiness().RunASingleTimeOnTwoLocks(Rystem.Installation.Default).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoLocks2RedisCache()
        {
            await new DealWithLockBusiness().RunASingleTimeOnTwoLocks2(Rystem.Installation.Inst00).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoLocks2BlobStorage()
        {
            await new DealWithLockBusiness().RunASingleTimeOnTwoLocks2(Rystem.Installation.Default).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoRaceConditionsRedisCache()
        {
            await new DealWithConcurrencyBusiness().RunASingleTimeOnTwoRaces(Rystem.Installation.Inst00).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoRaceConditionsBlobStorage()
        {
            await new DealWithConcurrencyBusiness().RunASingleTimeOnTwoRaces(Rystem.Installation.Default).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoRaceConditions2RedisCache()
        {
            await new DealWithConcurrencyBusiness().RunASingleTimeOnTwoRaces2(Rystem.Installation.Inst00).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoRaceConditions2BlobStorage()
        {
            await new DealWithConcurrencyBusiness().RunASingleTimeOnTwoRaces2(Rystem.Installation.Default).NoContext();
        }
        [Fact]
        public async Task Runtime()
        {
            await new SecondDealingKey() { Key = string.Empty }.LockAsync(() => Task.CompletedTask);
        }
    }
}