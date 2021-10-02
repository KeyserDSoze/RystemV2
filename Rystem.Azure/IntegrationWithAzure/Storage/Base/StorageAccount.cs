using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Cosmos.Table;

namespace Rystem.Azure.Integration.Storage
{
    /// <summary>
    /// Leave AccountKey empty if you want to connect through the managed identity. Not valid for TableStorage.
    /// </summary>
    public sealed class StorageAccount : IRystemOptions
    {
        public string AccountName { get; init; }
        public string AccountKey { get; init; }
        public BlobClientOptions BlobClientOptions { get; init; }
        public QueueClientOptions QueueClientOptions { get; init; }
        public TableClientConfiguration TableClientConfiguration { get; init; }
        public string ConnectionString
            => string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net", AccountName, AccountKey);
        public bool UseKeyVault { get; }
        public KeyVaultValue KeyVaultValue { get; }

        public StorageAccount(KeyVaultValue keyVaultValue)
        {
            KeyVaultValue = keyVaultValue;
            UseKeyVault = true;
        }
        public StorageAccount(string accountName, string accountKey = default, BlobClientOptions blobClientOptions = default, QueueClientOptions queueClientOptions = default, TableClientConfiguration tableClientConfiguration = default)
        {
            AccountName = accountName;
            AccountKey = accountKey;
            BlobClientOptions = blobClientOptions;
            QueueClientOptions = queueClientOptions;
            TableClientConfiguration = tableClientConfiguration;
        }
    }
}