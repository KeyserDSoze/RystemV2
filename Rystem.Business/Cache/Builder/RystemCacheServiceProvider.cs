using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Business
{
    public sealed class RystemCacheServiceProvider<TCacheKey, TCache> : ServiceProvider
        where TCacheKey : ICacheKey<TCache>
    {
        internal RystemCacheServiceProvider(IServiceCollection services) : base(services) { }
        public AzureCacheServiceBuilder<TCacheKey, TCache> WithAzure(CacheConfiguration configuration = default, Installation installation = Installation.Default)
          => AndWithAzure(configuration, installation);
        public AzureCacheServiceBuilder<TCacheKey, TCache> AndWithAzure(CacheConfiguration configuration = default, Installation installation = Installation.Default)
          => new(installation, configuration, this);
        public RystemCacheServiceProvider<TCacheKey, TCache> WithMemory(CacheConfiguration configuration = default)
          => AndMemory(configuration);
        public RystemCacheServiceProvider<TCacheKey, TCache> AndMemory(CacheConfiguration configuration = default)
        {
            Services.Add(Installation.Memory, new ProvidedService(ServiceProviderType.InMemory, default, string.Empty, configuration ?? new CacheConfiguration(default)));
            return this;
        }
        public IServiceCollection Configure()
        {
            ServiceCollection.AddSingleton(new Options<ICacheManager<ICacheKey<TCache>, TCache>>(Services));
            ServiceCollection.AddSingleton<ICacheManager<ICacheKey<TCache>, TCache>, CacheManager<ICacheKey<TCache>, TCache>>();
            ServiceCollection.AddRystemFullyAddedCallback(() => ServiceLocator.GetService<ICacheManager<ICacheKey<TCache>, TCache>>().WarmUpAsync());
            return ServiceCollection;
        }
    }
}