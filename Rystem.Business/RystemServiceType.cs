using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    internal enum RystemServiceType
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