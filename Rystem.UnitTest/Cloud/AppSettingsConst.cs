using Microsoft.Extensions.Configuration;
using Rystem.Cloud;
using Rystem.Cloud.Azure;

namespace Rystem.UnitTest.Cloud
{
    internal sealed class AppSettingsConst
    {
        public static AzureAadAppRegistration AzureAadAppRegistration;
        public static void Load()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var aad = config.GetSection("AzureAad");
            ServiceLocator
                .Create()
                .AddAzureManager(new AzureAadAppRegistration(aad["ClientId"], aad["ClientSecret"], aad["TenantId"]))
                .FinalizeWithoutDependencyInjection();
        }
    }
}
