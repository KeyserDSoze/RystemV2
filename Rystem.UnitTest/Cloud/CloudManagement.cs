using Rystem.Cloud;
using Rystem.Cloud.Azure;
using Rystem.Text;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Cloud
{
    public class CloudManagement
    {
        static CloudManagement()
        {
            AppSettingsConst.Load();
        }
        [Fact]
        public async Task IsOk()
        {
            MyCloud myCloud = new();
            var tenant = await myCloud.GetPreviousMonthTenantAsync().NoContext();
            string value = tenant.ToJson();
        }
        public class MyCloud : ICloud
        {
            public CloudBuilder Configure()
            {
                return CloudBuilder.Create()
                    .WithAzure(AppSettingsConst.AzureAadAppRegistration);
            }
        }
    }
}
