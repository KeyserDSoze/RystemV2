using Azure.Messaging.EventHubs;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rystem.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.UnitTest
{
    internal static class AzureConst
    {
        public static IServiceCollection Load()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var storage = config.GetSection("Storage");
            var eventHub = config.GetSection("EventHub");
            var serviceBus = config.GetSection("ServiceBus");
            var redisCache = config.GetSection("RedisCache");
            var cosmos = config.GetSection("Cosmos");
            return RystemBusiness
              .WithAzure()
              .AddStorage(new Azure.Integration.Storage.StorageAccount(storage["Name"], storage["Key"]))
              .AddEventHub(new Azure.Integration.Message.EventHubAccount(eventHub["FullyQualifiedName"], eventHub["AccessKey"], new Azure.Integration.Storage.StorageAccount(storage["Name"], storage["Key"])))
              .AddServiceBus(new Azure.Integration.Message.ServiceBusAccount(serviceBus["FullyQualifiedName"], serviceBus["AccessKey"]))
              .AddRedisCache(new Azure.Integration.Cache.RedisCacheAccount(redisCache["ConnectionString"], TimeSpan.FromHours(1), 4))
              .AddCosmos(new Azure.Integration.Cosmos.CosmosAccount(cosmos["AccountName"], cosmos["AccountKey"]))
              .EndConfiguration();
        }
    }
}