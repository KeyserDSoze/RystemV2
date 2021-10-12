using Rystem.Cloud;
using Rystem.Text;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Cloud
{
    public class CloudManagement
    {
        private readonly CloudManager Manager = ServiceLocator.GetService<CloudManager>();
        static CloudManagement()
        {
            AppSettingsConst.Load();
        }
        [Fact]
        public async Task IsOk()
        {
            var cloudManager = await Manager.GetManagerAsync().NoContext();
            var tenant = await cloudManager.GetTenantByMonthAsync(DateTime.UtcNow.AddMonths(-1), ManagementDeepRequest.Monitoring, false).NoContext();
            string value = tenant.Tenant.ToJson();
        }
    }
}
