using Microsoft.Extensions.DependencyInjection;
using Rystem.Security.Cryptography;

namespace Rystem
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAes(this IServiceCollection services, AesCryptoOptions options)
        {
            Crypto.Aes.Configure(options);
            return services;
        }
    }
}
