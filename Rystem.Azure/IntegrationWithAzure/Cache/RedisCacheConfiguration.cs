using System;

namespace Rystem.Azure.Integration.Cache
{
    public sealed record RedisCacheConfiguration(string Prefix, TimeSpan ExpiringDefault, int NumberOfClients = 1) : Configuration(Prefix)
    {
        public RedisCacheConfiguration() : this(string.Empty, default) { }
    }
}