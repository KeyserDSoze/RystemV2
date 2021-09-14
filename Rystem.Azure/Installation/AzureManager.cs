using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Secrets;
using Rystem.Azure.Integration.Storage;
using Rystem.Azure.Integration.Cosmos;
using System.Collections.Generic;

namespace Rystem.Azure
{
    public sealed class AzureManager
    {
        public AzureManager() { }
        private AzureFactory factory;
        public AzureFactory Factory => factory ??= new AzureFactory(this);
        public Dictionary<string, RedisCacheOptions> RedisCaches { get; } = new();
        public Dictionary<string, EventHubOptions> EventHubs { get; } = new();
        public Dictionary<string, ServiceBusOptions> ServiceBuses { get; } = new();
        public Dictionary<string, KeyVaultOptions> KeyVaults { get; } = new();
        public Dictionary<string, StorageOptions> Storages { get; } = new();
        public Dictionary<string, CosmosOptions> Cosmos { get; } = new();
    }
    public static class AzureManagerExtensions
    {
        public static AzureBuilder WithAzure(this IServiceCollection services)
            => RystemServices.AzureBuilder = new(RystemManager.ServiceCollection = services ?? new ServiceCollection());
    }
}