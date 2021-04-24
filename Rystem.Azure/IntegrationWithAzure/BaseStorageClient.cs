using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Azure.IntegrationWithAzure
{
    internal abstract class BaseStorageClient
    {
        private protected readonly string ConnectionString;
        public BaseStorageClient(string accountName, string accountKey)
        {
            ConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net", accountName, accountKey);
        }

        private protected readonly string AccountName;
        public BaseStorageClient(string accountName)
        {
            AccountName = accountName;
        }
    }
}
