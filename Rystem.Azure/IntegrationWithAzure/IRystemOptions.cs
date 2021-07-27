using Rystem.Azure.Integration.Secrets;
using System.Threading.Tasks;

namespace Rystem.Azure
{
    public interface IRystemOptions
    {
        bool UseKeyVault { get; }
        string ConnectionString { get; }
        KeyVaultValue KeyVaultValue { get; }
        internal async Task<string> GetConnectionStringAsync()
        {
            if (!UseKeyVault)
                return ConnectionString;
            else
                return (await RystemServices.AzureBuilder.AzureFactory.KeyVault(KeyVaultValue.ServiceKey).GetSecretAsync(KeyVaultValue.Key).NoContext()).Value;
        }
    }
}