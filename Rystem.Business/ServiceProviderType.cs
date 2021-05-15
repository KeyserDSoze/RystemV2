using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    internal enum ServiceProviderType
    {
        InMemory,
        AzureKeyVault,
        AzureTableStorage,
        AzureBlobStorage,
        AzureQueueStorage,
        AzureEventHub,
        AzureServiceBus,
        AzureRedisCache
    }
}