using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Concurrency
{
    public sealed class RystemDistributedServiceProvider
    {
        private RystemDistributedServiceProvider() { }
        internal static readonly RystemDistributedServiceProvider Instance = new();
        public static RystemDistributedServiceProvider<T> Configure<T>(T entity)
            where T : IDistributedConcurrencyKey
            => new(ServiceLocatorAtRuntime.PrepareToAddNewService());
    }
    public sealed class RystemDistributedServiceProvider<T> : ServiceProvider
        where T : IDistributedConcurrencyKey
    {
        internal RystemDistributedServiceProvider(IServiceCollection services) : base(services)
        {
        }
        public AzureDistributedServiceBuilder<T> WithAzure(Installation installation = Installation.Default)
          => AndWithAzure(installation);
        public AzureDistributedServiceBuilder<T> AndWithAzure(Installation installation = Installation.Default)
          => new(installation, this);
        public RystemDistributedServiceProvider<T> WithMemory()
          => AndMemory();
        public RystemDistributedServiceProvider<T> AndMemory()
        {
            Services.Add(Installation.Memory, new ProvidedService(ServiceProviderType.InMemory, default, string.Empty, default));
            return this;
        }
        public IServiceCollection Configure()
        {
            ServiceCollection.AddSingleton(new Options<IDistributedManager<T>>(Services));
            ServiceCollection.AddSingleton<IDistributedManager<T>, DistributedManager<T>>();
            ServiceCollection.AddRystemFullyAddedCallback(() => ServiceLocator.GetService<IDistributedManager<T>>().WarmUpAsync());
            return ServiceCollection;
        }
        public RystemDistributedServiceProvider ConfigureAfterBuild()
        {
            Configure();
            ServiceLocatorAtRuntime.Rebuild();
            return RystemDistributedServiceProvider.Instance;
        }
    }
}