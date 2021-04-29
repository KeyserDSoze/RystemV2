using Microsoft.Extensions.DependencyInjection;
using Rystem.Azure.IntegrationWithAzure.Cache;
using Rystem.Azure.IntegrationWithAzure.Message;
using Rystem.Azure.IntegrationWithAzure.Secrets;
using Rystem.Azure.IntegrationWithAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Azure.Installation
{
    public class AzureManager
    {
        private AzureManager() { }
        public static AzureManager Instance { get; } = new AzureManager();
        public Dictionary<string, RedisCacheIntegration> RedisCaches { get; } = new();
        public Dictionary<string, EventHubOptions> EventHubs { get; } = new();
        public Dictionary<string, ServiceBusOptions> ServiceBuses { get; } = new();
        public Dictionary<string, KeyVaultIntegration> KeyVaults { get; } = new();
        public Dictionary<string, StorageOptions> Storages { get; } = new();
    }
    public class AzureBuilder
    {
        private readonly IServiceCollection Services;
        public AzureBuilder() { }
        public AzureBuilder(IServiceCollection services)
        {
            Services = services;
        }
        /// <summary>
        /// Add Azure storage service
        /// </summary>
        /// <param name="options">Use only account name if you want to connect through the managed identity.</param>
        /// <param name="labelKey">A specific key that you will use during your object configuration.</param>
        /// <returns></returns>
        public AzureBuilder AddStorage(StorageOptions options, string labelKey = null)
            => Add(AzureManager.Instance.Storages, options, labelKey);
        public AzureBuilder AddMessage(EventHubOptions options, string labelKey = null)
            => Add(AzureManager.Instance.EventHubs, options, labelKey);
        public AzureBuilder AddMessage(ServiceBusOptions options, string labelKey = null)
            => Add(AzureManager.Instance.ServiceBuses, options, labelKey);
        public AzureBuilder AddCache(RedisCacheOptions options, string labelKey = null)
            => Add(AzureManager.Instance.RedisCaches, new RedisCacheIntegration(options), labelKey);
        public AzureBuilder AddKeyVault(KeyVaultOptions options, string labelKey = null)
            => Add(AzureManager.Instance.KeyVaults, new KeyVaultIntegration(options), labelKey);
        private AzureBuilder Add<T>(Dictionary<string, T> dictionary, T options, string labelKey = null)
        {
            if (labelKey == null)
                labelKey = string.Empty;
            if (dictionary.ContainsKey(labelKey))
                throw new ArgumentException($"Key {labelKey} already installed for {typeof(T).Name}.");
            dictionary.Add(labelKey, options);
            return this;
        }
        public IServiceCollection Build()
        {
            return this.Services;
        }
    }
    public static class AzureManagerExtensions
    {
        public static AzureBuilder ConfigureAzureServices(this IServiceCollection services)
            => new(services);
    }
}
