using Rystem.Azure;
using Rystem.Cloud.Azure;
using Rystem.Text;
using System.Threading.Tasks;

namespace Rystem.Cloud
{
   
    internal sealed record CloudOptions(CloudType Type, object Options)
    {
        public async Task<AzureAadAppRegistration> GetAzureAadAppRegistrationAsync()
        {
            if (Options is AzureAadAppRegistration options)
                return options;
            else if (Options is KeyVaultValue keyVaultValue)
                return (await ServiceLocator.GetService<AzureManager>().KeyVault(keyVaultValue.ServiceKey).GetSecretAsync(keyVaultValue.Key).NoContext()).Value.FromJson<AzureAadAppRegistration>();
            else
                return default;
        }
    }
}