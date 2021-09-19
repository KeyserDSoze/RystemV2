using Azure.Messaging.ServiceBus;

namespace Rystem.Azure.Integration.Message
{
    //for instance something.servicebus.windows.net
    /// <summary>
    /// Leave ConnectionString empty if you want to connect through the managed identity
    /// </summary>
    public sealed record ServiceBusAccount(string FullyQualifiedName, string AccessKey, ServiceBusProcessorOptions Options = default) : IRystemOptions
    {
        public string ConnectionString => $"Endpoint=sb://{FullyQualifiedName}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={AccessKey}";
        public bool UseKeyVault { get; }
        public KeyVaultValue KeyVaultValue { get; }

        public ServiceBusAccount(KeyVaultValue keyVaultValue, ServiceBusProcessorOptions options = default)
            : this(string.Empty, string.Empty, options)
        {
            KeyVaultValue = keyVaultValue;
            UseKeyVault = true;
        }
    }
}