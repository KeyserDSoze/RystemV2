using Azure.Messaging.EventHubs;
using Rystem.Azure.Integration.Storage;

namespace Rystem.Azure.Integration.Message
{
    /// <summary>
    /// Leave ConnectionString empty if you want to connect through the managed identity
    /// </summary>
    public sealed record EventHubAccount(string FullyQualifiedName, string AccessKey, StorageAccount StorageOptions = default, EventProcessorClientOptions ProcessorOptions = default) : IRystemOptions
    {
        public string ConnectionString => $"Endpoint=sb://{FullyQualifiedName}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={AccessKey}";
        public bool UseKeyVault { get; }
        public KeyVaultValue KeyVaultValue { get; }

        public EventHubAccount(KeyVaultValue keyVaultValue, StorageAccount storageOptions = default, EventProcessorClientOptions processorOptions = default)
            : this(string.Empty, string.Empty, storageOptions, processorOptions)
        {
            KeyVaultValue = keyVaultValue;
            UseKeyVault = true;
        }
    }
}