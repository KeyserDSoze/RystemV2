﻿using Microsoft.Extensions.DependencyInjection;
using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Secrets;
using Rystem.Azure.Integration.Storage;
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
        public Dictionary<string, RedisCacheOptions> RedisCaches { get; } = new();
        public Dictionary<string, EventHubOptions> EventHubs { get; } = new();
        public Dictionary<string, ServiceBusOptions> ServiceBuses { get; } = new();
        public Dictionary<string, KeyVaultOptions> KeyVaults { get; } = new();
        public Dictionary<string, StorageOptions> Storages { get; } = new();
    }
    public class AzureBuilder
    {
        private readonly IServiceCollection Services;
        internal AzureBuilder() { }
        internal AzureBuilder(IServiceCollection services)
        {
            Services = services;
        }
        /// <summary>
        /// Add Azure storage service
        /// </summary>
        /// <param name="options">Use only account name if you want to connect through the managed identity.</param>
        /// <param name="serviceKey">A specific key that you will use during your object configuration.</param>
        /// <returns></returns>
        public AzureBuilder AddStorage(StorageOptions options, string serviceKey = null)
            => Add(AzureManager.Instance.Storages, options, serviceKey);
        public AzureBuilder AddMessage(EventHubOptions options, string serviceKey = null)
            => Add(AzureManager.Instance.EventHubs, options, serviceKey);
        public AzureBuilder AddMessage(ServiceBusOptions options, string serviceKey = null)
            => Add(AzureManager.Instance.ServiceBuses, options, serviceKey);
        public AzureBuilder AddCache(RedisCacheOptions options, string serviceKey = null)
            => Add(AzureManager.Instance.RedisCaches, options, serviceKey);
        public AzureBuilder AddKeyVault(KeyVaultOptions options, string serviceKey = null)
            => Add(AzureManager.Instance.KeyVaults, options, serviceKey);
        private AzureBuilder Add<T>(Dictionary<string, T> dictionary, T options, string serviceKey = null)
        {
            if (serviceKey == null)
                serviceKey = string.Empty;
            if (dictionary.ContainsKey(serviceKey))
                throw new ArgumentException($"Key {serviceKey} already installed for {typeof(T).Name}.");
            dictionary.Add(serviceKey, options);
            return this;
        }
        public IServiceCollection Build()
        {
            return this.Services;
        }
    }
    public static class AzureManagerExtensions
    {
        public static AzureBuilder WithAzure(this IServiceCollection services)
            => new(services);
    }
}
