using System;

namespace Rystem.Business
{
    public sealed record CacheConfiguration(TimeSpan ExpiringDefault) : Configuration(string.Empty);
}