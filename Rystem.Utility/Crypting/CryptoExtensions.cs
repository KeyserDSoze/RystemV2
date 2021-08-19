﻿using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Security.Cryptography
{
    public static class CryptoExtensions
    {
        public static IServiceCollection AddAes(this IServiceCollection services, AesCryptoOptions options)
        {
            Crypto.Aes.Configure(options);
            return services;
        }
    }
}
