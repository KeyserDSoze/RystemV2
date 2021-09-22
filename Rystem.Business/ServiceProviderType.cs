namespace Rystem.Business
{
    public enum ServiceProviderType
    {
        InMemory,
        AzureKeyVault,
        AzureTableStorage,
        AzureBlockBlobStorage,
        AzureAppendBlobStorage,
        AzureQueueStorage,
        AzureEventHub,
        AzureServiceBus,
        AzureRedisCache,
        AzureCosmosNoSql
    }
}