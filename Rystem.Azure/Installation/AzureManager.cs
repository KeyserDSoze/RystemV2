using Microsoft.Extensions.DependencyInjection;
using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Secrets;
using Rystem.Azure.Integration.Storage;
using System.Collections.Generic;

namespace Rystem.Azure.Installation
{
    public class AzureManager
    {
        private AzureManager() { }
        public static AzureManager Instance { get; } = new AzureManager();
        public AzureFactory Factory { get; } = new AzureFactory();
        public Dictionary<string, RedisCacheOptions> RedisCaches { get; } = new();
        public Dictionary<string, EventHubOptions> EventHubs { get; } = new();
        public Dictionary<string, ServiceBusOptions> ServiceBuses { get; } = new();
        public Dictionary<string, KeyVaultOptions> KeyVaults { get; } = new();
        public Dictionary<string, StorageOptions> Storages { get; } = new();
    }
    public static class AzureManagerExtensions
    {
        public static AzureBuilder WithAzure(this IServiceCollection services)
            => new(services);
    }
}