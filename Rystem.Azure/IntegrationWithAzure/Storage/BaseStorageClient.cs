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
    public sealed record StorageOptions(string AccountName, string AccountKey = null)
    {
        public string GetConnectionString()
            => string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net", AccountName, AccountKey);
    }
    public abstract class BaseStorageClient
    {
        private protected StorageOptions Options;
        public BaseStorageClient(StorageOptions options)
        {
            Options = options;
        }
    }
}
