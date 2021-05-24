using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    internal enum ServiceProviderType
    {
        InMemory,
        InMemory2,
        AzureKeyVault,
        AzureTableStorage,
        AzureBlobStorage,
        AzureQueueStorage,
        AzureEventHub,
        AzureServiceBus,
        AzureRedisCache
    }
}