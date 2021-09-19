using System;

namespace Rystem.Azure.Integration.Cache
{
    public sealed record RedisCacheAccount(string ConnectionString, TimeSpan ExpiringDefault, int NumberOfClients = 1) : IRystemOptions
    {
        public bool UseKeyVault { get; }
        public KeyVaultValue KeyVaultValue { get; }

        public RedisCacheAccount(KeyVaultValue keyVaultValue, TimeSpan expiringDefault, int numberOfClients = 1)
            : this(string.Empty, expiringDefault, numberOfClients)
        {
            KeyVaultValue = keyVaultValue;
            UseKeyVault = true;
        }
    }
}