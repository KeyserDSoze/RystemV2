using Rystem.Cloud;
using Rystem.Text;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Cloud
{
    public class CloudManagement
    {
        private readonly ICloudManagement Manager = ServiceLocator.GetService<ICloudManagement>();
        static CloudManagement()
        {
            AppSettingsConst.Load();
        }
        [Fact]
        public async Task IsOk()
        {
            var tenant = await Manager.GetTenantByMonthAsync(DateTime.UtcNow.AddMonths(-1), ManagementDeepRequest.Monitoring, false).NoContext();
            string value = tenant.ToJson();
        }
    }
}
