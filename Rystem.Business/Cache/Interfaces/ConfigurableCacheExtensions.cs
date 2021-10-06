namespace Rystem.Business
{
    public static class ConfigurableCacheExtensions
    {
        public static RystemCacheServiceProvider<TCacheKey, TCache> StartConfiguration<TCacheKey, TCache>(this TCacheKey configurableData)
            where TCacheKey : ICacheKey<TCache>
            => RystemCacheServiceProvider
                    .Configure<TCacheKey, TCache>(configurableData);
    }
}