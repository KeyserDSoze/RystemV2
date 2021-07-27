using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Secrets;
using Rystem.Azure.Integration.Storage;
using System;
using System.Collections.Generic;

namespace Rystem.Azure
{
    public class AzureBuilder
    {
        private readonly IServiceCollection Services;
        internal readonly AzureFactory AzureFactory = new(new AzureManager());
        internal AzureBuilder(IServiceCollection services)
        {
            Services = services.AddSingleton<RystemServices>();
        }
        /// <summary>
        /// Add Azure storage service
        /// </summary>
        /// <param name="options">Use only account name if you want to connect through the managed identity.</param>
        /// <param name="serviceKey">A specific key that you will use during your object configuration.</param>
        /// <returns></returns>
        public AzureBuilder AddStorage(StorageOptions options, string serviceKey = default)
            => Add(AzureFactory.Manager.Storages, options, serviceKey);
        public AzureBuilder AddMessage(EventHubOptions options, string serviceKey = default)
            => Add(AzureFactory.Manager.EventHubs, options, serviceKey);
        public AzureBuilder AddMessage(ServiceBusOptions options, string serviceKey = default)
            => Add(AzureFactory.Manager.ServiceBuses, options, serviceKey);
        public AzureBuilder AddCache(RedisCacheOptions options, string serviceKey = default)
        {
            Add(AzureFactory.Manager.RedisCaches, options, serviceKey);
            return this;
        }
        public AzureBuilder AddKeyVault(KeyVaultOptions options, string serviceKey = default)
        {
            Add(AzureFactory.Manager.KeyVaults, options, serviceKey);
            return this;
        }
        private AzureBuilder Add<T>(Dictionary<string, T> dictionary, T options, string serviceKey = default)
        {
            if (serviceKey == default)
                serviceKey = string.Empty;
            if (dictionary.ContainsKey(serviceKey))
                throw new ArgumentException($"Key {serviceKey} already installed for {options.GetType().Name}.");
            dictionary.Add(serviceKey, options);
            return this;
        }
        public IServiceCollection Build()
            => Services;
    }
}