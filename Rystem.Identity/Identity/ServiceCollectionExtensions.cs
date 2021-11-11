using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Rystem.Business;
using Rystem.Identity;
using System;
using Rystem.Azure.Integration.Storage;
using Rystem.Azure.Integration.Cosmos;
using Microsoft.Azure.Cosmos;
using Rystem.Identity.External;

namespace Rystem
{
    public static partial class ServiceCollectionExtensions
    {
        private static IdentityBuilder AddCustomStorageIdentity<TUser, TRole>(this IServiceCollection services,
            Action<RystemIdentityOptions> rystemOptionConfiguration,
            Action<IdentityOptions> options,
            string serviceKey,
            ServiceProviderType type)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
        {
            var rystemOptions = new RystemIdentityOptions()
            {
                AccountTableName = "Accounts",
                RoleTableName = "Roles",
                ClaimsTableName = "AccountClaims",
                AccountRolesTableName = "AccountRoles",
                ExternalIsAutoregistered = true,
            };
            rystemOptionConfiguration(rystemOptions);
            var builder = services
               .UseDocumentOn<TUser>()
               .AddCustomStorage(rystemOptions.AccountTableName, serviceKey, type)
                   .WithPrimaryKey(x => x.NormalizedUserName)
                   .WithSecondaryKey(x => x.Id)
                   .WithoutTimestamp()
                .AddCustomStorage($"{rystemOptions.AccountTableName}ById", serviceKey, type, Installation.Inst00)
                   .WithPrimaryKey(x => x.Id)
                   .WithSecondaryKey(x => x.NormalizedUserName)
                   .WithoutTimestamp()
                   .Configure()
               .UseDocumentOn<TRole>()
                   .AddCustomStorage(rystemOptions.RoleTableName, serviceKey, type)
                   .WithPrimaryKey(x => x.NormalizedName)
                   .WithSecondaryKey(x => x.Id)
                   .WithoutTimestamp()
                   .Configure()
               .UseDocumentOn<ClaimsForAccount>()
                   .AddCustomStorage(rystemOptions.ClaimsTableName, serviceKey, type)
                   .WithPrimaryKey(x => x.Id)
                   .WithSecondaryKey(x => x.ClaimId)
                   .WithTimestamp(x => x.CreatedAt)
                   .Configure()
               .UseDocumentOn<RolesForAccount>()
                   .AddCustomStorage(rystemOptions.AccountRolesTableName, serviceKey, type)
                   .WithPrimaryKey(x => x.Id)
                   .WithSecondaryKey(x => x.Role)
                   .WithTimestamp(x => x.CreatedAt)
                   .Configure()
               .AddIdentity<IdentityUser, IdentityRole>(options)
               .AddUserStore<RystemIdentityStore>()
               .AddRoleStore<RystemRoleStore>();
            if (rystemOptions.HasMicrosoftAsExternalLogin)
            {
                services.AddAuthentication()
                    .AddMicrosoftAccount(configureOptions =>
                    {
                        rystemOptions.MicrosoftConfigureOptions(configureOptions);
                        if (rystemOptions.ExternalIsAutoregistered)
                            configureOptions.Events.OnCreatingTicket +=
                                (ticket) => BasicAutoRegistrationManager.RegisterAsync<IdentityUser>(ticket, "Microsoft");
                    });
            }
            if (rystemOptions.HasGoogleAsExternalLogin)
            {
                services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        rystemOptions.GoogleConfigureOptions(options);
                        if (rystemOptions.ExternalIsAutoregistered)
                            options.Events.OnCreatingTicket +=
                                (ticket) => BasicAutoRegistrationManager.RegisterAsync<IdentityUser>(ticket, "Google");
                    });
            }
            if (rystemOptions.HasFacebookAsExternalLogin)
            {
                services.AddAuthentication()
                    .AddFacebook(options =>
                    {
                        rystemOptions.FacebookConfigureOptions(options);
                        if (rystemOptions.ExternalIsAutoregistered)
                            options.Events.OnCreatingTicket +=
                                (ticket) => BasicAutoRegistrationManager.RegisterAsync<IdentityUser>(ticket, "Facebook");
                    });
            }
            return builder;
        }
        private static RystemDocumentServicePrimaryKey<T> AddCustomStorage<T>(this RystemDocumentServiceProvider<T> builder, string name, string serviceKey, ServiceProviderType type, Installation installation = Installation.Default)
            => type switch
            {
                ServiceProviderType.AzureTableStorage => builder.WithAzure(installation).WithTableStorage(new TableStorageConfiguration(name), serviceKey),
                ServiceProviderType.AzureBlockBlobStorage => builder.WithAzure(installation).WithBlobStorage(new BlobStorageConfiguration(name), serviceKey),
                ServiceProviderType.AzureCosmosNoSql => builder.WithAzure(installation).WithCosmosNoSql(new CosmosConfiguration(new ContainerProperties(name, string.Empty)), serviceKey),
                _ => throw new ArgumentException($"{nameof(type)} is not supported."),
            };
        public static IServiceCollection AddIdentityNotification<T>(this IdentityBuilder builder)
            where T : class, IIdentityNotifications<IdentityUser, IdentityRole>
            => builder.Services.AddSingleton<IIdentityNotifications<IdentityUser, IdentityRole>, T>();

        public static IdentityBuilder AddTableStorageIdentity<TUser, TRole>(this IServiceCollection services,
            Action<RystemIdentityOptions> rystemOptionConfiguration,
            Action<IdentityOptions> options = default,
            string serviceKey = default)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            => services
                .AddCustomStorageIdentity<TUser, TRole>(rystemOptionConfiguration,
                options,
                serviceKey,
                ServiceProviderType.AzureTableStorage);


        public static IdentityBuilder AddTableStorageIdentity(this IServiceCollection services,
            Action<RystemIdentityOptions> rystemOptionConfiguration,
            Action<IdentityOptions> options = default,
            string serviceKey = default)
            => services
                .AddTableStorageIdentity<IdentityUser, IdentityRole>(rystemOptionConfiguration, options, serviceKey);
        public static IdentityBuilder AddCosmosNoSqlIdentity<TUser, TRole>(this IServiceCollection services,
            Action<RystemIdentityOptions> rystemOptionConfiguration,
            Action<IdentityOptions> options = default,
            string serviceKey = default)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            => services
                .AddCustomStorageIdentity<TUser, TRole>(rystemOptionConfiguration,
                options,
                serviceKey,
                ServiceProviderType.AzureCosmosNoSql);
        public static IdentityBuilder AddCosmosNoSqlIdentity(this IServiceCollection services,
            Action<RystemIdentityOptions> rystemOptionConfiguration,
            Action<IdentityOptions> options = default,
            string serviceKey = default)
            => services.AddCosmosNoSqlIdentity<IdentityUser, IdentityRole>(rystemOptionConfiguration, options, serviceKey);
        public static IdentityBuilder AddBlobStorageIdentity<TUser, TRole>(this IServiceCollection services,
            Action<RystemIdentityOptions> rystemOptionConfiguration,
            Action<IdentityOptions> options = default,
            string serviceKey = default)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            => services
                .AddCustomStorageIdentity<TUser, TRole>(rystemOptionConfiguration,
                options,
                serviceKey,
                ServiceProviderType.AzureBlockBlobStorage);
        public static IdentityBuilder AddBlobStorageIdentity(this IServiceCollection services,
            Action<RystemIdentityOptions> rystemOptionConfiguration,
            Action<IdentityOptions> options = default,
            string serviceKey = default)
            => services.AddBlobStorageIdentity<IdentityUser, IdentityRole>(rystemOptionConfiguration, options, serviceKey);
    }
}