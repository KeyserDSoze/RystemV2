using Rystem.Azure.Integration.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Azure.Integration.Storage
{
    /// <summary>
    /// Leave AccountKey empty if you want to connect through the managed identity. Not valid for TableStorage.
    /// </summary>
    public sealed record StorageAccount(string AccountName, string AccountKey = default) : IRystemOptions
    {
        public string ConnectionString
            => string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net", AccountName, AccountKey);
        public bool UseKeyVault { get; }
        public KeyVaultValue KeyVaultValue { get; }

        public StorageAccount(KeyVaultValue keyVaultValue)
            : this(default, default)
        {
            KeyVaultValue = keyVaultValue;
            UseKeyVault = true;
        }
    }
}