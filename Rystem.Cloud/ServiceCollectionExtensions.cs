using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rystem.Azure;
using Rystem.Cloud;
using Rystem.Cloud.Azure;
using System;

namespace Rystem
{
    public static class ServiceCollectionExtensions
    {
        private static readonly CloudManager Manager = new();
        public static IServiceCollection AddAzureManager(this IServiceCollection services, AzureAadAppRegistration appRegistration, string key = "")
            => services.AddAzureManager((object)appRegistration, key);
        public static IServiceCollection AddAzureManager(this IServiceCollection services, KeyVaultValue keyVaultValue, string key = "") 
            => services.AddAzureManager((object)keyVaultValue, key);
        private static IServiceCollection AddAzureManager(this IServiceCollection services, object options, string key)
        {
            services.TryAddSingleton(Manager);
            services.AddHttpClient($"rystem.cloud.azure.{key}", configuration =>
            {
                configuration.Timeout = TimeSpan.FromMinutes(3);
            });
            Manager.Add(key, new CloudOptions(CloudType.Azure, options));
            return services;
        }
    }
}