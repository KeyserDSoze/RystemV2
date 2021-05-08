using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    internal enum ServiceProviderType
    {
        AzureKeyVault,
        AzureTableStorage,
        AzureBlobStorage,
        AzureQueueStorage,
        AzureEventHub,
        AzureServiceBus,
        AzureRedisCache
    }
}