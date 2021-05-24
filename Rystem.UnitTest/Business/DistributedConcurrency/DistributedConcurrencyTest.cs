using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Business.DistributedConcurrency
{
    public class DistributedConcurrencyTest
    {
        static DistributedConcurrencyTest()
        {
            AzureConst.Load();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoLocksRedisCache()
        {
            await new DealWithLockBusiness().RunASingleTimeOnTwoLocks(Rystem.Business.Installation.Inst00).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoLocksBlobStorage()
        {
            await new DealWithLockBusiness().RunASingleTimeOnTwoLocks(Rystem.Business.Installation.Default).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoLocks2RedisCache()
        {
            await new DealWithLockBusiness().RunASingleTimeOnTwoLocks2(Rystem.Business.Installation.Inst00).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoLocks2BlobStorage()
        {
            await new DealWithLockBusiness().RunASingleTimeOnTwoLocks2(Rystem.Business.Installation.Default).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoRaceConditionsRedisCache()
        {
            await new DealWithConcurrencyBusiness().RunASingleTimeOnTwoRaces(Rystem.Business.Installation.Inst00).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoRaceConditionsBlobStorage()
        {
            await new DealWithConcurrencyBusiness().RunASingleTimeOnTwoRaces(Rystem.Business.Installation.Default).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoRaceConditions2RedisCache()
        {
            await new DealWithConcurrencyBusiness().RunASingleTimeOnTwoRaces2(Rystem.Business.Installation.Inst00).NoContext();
        }
        [Fact]
        public async Task RunASingleTimeOnTwoRaceConditions2BlobStorage()
        {
            await new DealWithConcurrencyBusiness().RunASingleTimeOnTwoRaces2(Rystem.Business.Installation.Default).NoContext();
        }
    }
}
