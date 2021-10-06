namespace Rystem.Business
{
    public interface IConfigurableCacheKey<TInstance> : ICacheKey<TInstance>, IConfigurable<RystemCacheServiceProvider>
    {
    }
}