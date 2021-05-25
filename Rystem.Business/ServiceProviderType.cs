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
        AzureBlockBlobStorage,
        AzureAppendBlobStorage,
        AzureQueueStorage,
        AzureEventHub,
        AzureServiceBus,
        AzureRedisCache
    }
}