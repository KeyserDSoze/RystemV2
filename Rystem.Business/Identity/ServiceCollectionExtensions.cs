using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Rystem.Business;
using Rystem.Business.Identity;
using System;
using Rystem.Azure.Integration.Storage;
using Rystem.Azure.Integration.Cosmos;
using Microsoft.Azure.Cosmos;

namespace Rystem
{
    public static partial class ServiceCollectionExtensions
    {
        private static IdentityBuilder AddCustomStorageIdentity(this IServiceCollection services,
            Action<IdentityOptions> options,
            string serviceKey,
            ServiceProviderType type)
        {
            return services
               .UseDocumentOn<IdentityUser>()
               .AddCustomStorage("Accounts", serviceKey, type)
                   .WithPrimaryKey(x => x.NormalizedUserName)
                   .WithSecondaryKey(x => x.Id)
                   .WithoutTimestamp()
                   .Configure()
               .UseDocumentOn<IdentityRole>()
                   .AddCustomStorage("Roles", serviceKey, type)
                   .WithPrimaryKey(x => x.Id)
                   .WithSecondaryKey(x => x.NormalizedName)
                   .WithoutTimestamp()
                   .Configure()
               .UseDocumentOn<ClaimsForAccount>()
                   .AddCustomStorage("AccountClaims", serviceKey, type)
                   .WithPrimaryKey(x => x.Id)
                   .WithSecondaryKey(x => x.ClaimId)
                   .WithTimestamp(x => x.CreatedAt)
                   .Configure()
               .UseDocumentOn<RolesForAccount>()
                   .AddCustomStorage("AccountRoles", serviceKey, type)
                   .WithPrimaryKey(x => x.Id)
                   .WithSecondaryKey(x => x.Role)
                   .WithTimestamp(x => x.CreatedAt)
                   .Configure()
               .AddIdentity<IdentityUser, IdentityRole>(options)
               .AddUserStore<RystemIdentityStore>()
               .AddRoleStore<RystemRoleStore>();
        }
        private static RystemDocumentServicePrimaryKey<T> AddCustomStorage<T>(this RystemDocumentServiceProvider<T> builder, string name, string serviceKey, ServiceProviderType type)
            => type switch
            {
                ServiceProviderType.AzureTableStorage => builder.WithAzure().WithTableStorage(new TableStorageConfiguration(name), serviceKey),
                ServiceProviderType.AzureBlockBlobStorage => builder.WithAzure().WithBlobStorage(new BlobStorageConfiguration(name), serviceKey),
                ServiceProviderType.AzureCosmosNoSql => builder.WithAzure().WithCosmosNoSql(new CosmosConfiguration(new ContainerProperties(name, string.Empty)), serviceKey),
                _ => throw new ArgumentException($"{nameof(type)} is not supported."),
            };
        public static IServiceCollection AddIdentityNotification<T>(this IdentityBuilder builder)
            where T : class, IIdentityNotifications<IdentityUser, IdentityRole>
            => builder.Services.AddSingleton<IIdentityNotifications<IdentityUser, IdentityRole>, T>();

        public static IdentityBuilder AddTableStorageIdentity(this IServiceCollection services,
            Action<IdentityOptions> options = default,
            string serviceKey = default)
            => services
                .AddCustomStorageIdentity(options,
                serviceKey,
                ServiceProviderType.AzureTableStorage);
        public static IdentityBuilder AddCosmosNoSqlIdentity(this IServiceCollection services,
            Action<IdentityOptions> options = default,
            string serviceKey = default)
            => services
                .AddCustomStorageIdentity(options,
                serviceKey,
                ServiceProviderType.AzureCosmosNoSql);
        public static IdentityBuilder AddBlobStorageIdentity(this IServiceCollection services,
            Action<IdentityOptions> options = default,
            string serviceKey = default)
            => services
                .AddCustomStorageIdentity(options,
                serviceKey,
                ServiceProviderType.AzureBlockBlobStorage);
    }
}