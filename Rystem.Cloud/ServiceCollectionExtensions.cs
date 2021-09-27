using Microsoft.Extensions.DependencyInjection;
using Rystem.Cloud.Azure;
using System;

namespace Rystem.Cloud
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureManager(this IServiceCollection services, AzureAadAppRegistration appRegistration)
        {
            services.AddSingleton(appRegistration);
            services.AddHttpClient("rystem.cloud.azure", configuration =>
            {
                configuration.Timeout = TimeSpan.FromMinutes(3);
            });
            services.AddSingleton<ICloudManagement, AzureCloudManager>();
            services.AddSingleton<AzureCloudManager>();
            return services;
        }
    }
}