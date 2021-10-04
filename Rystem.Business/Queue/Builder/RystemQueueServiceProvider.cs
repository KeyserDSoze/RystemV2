using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Business
{
    public sealed class RystemQueueServiceProvider
    {
        internal static readonly RystemQueueServiceProvider Instance = new();
        public static RystemQueueServiceProvider<T> Configure<T>(T entity)
            => new(ServiceLocatorAtRuntime.PrepareToAddNewService());
    }
    public sealed class RystemQueueServiceProvider<T> : ServiceProvider
    {
        internal RystemQueueServiceProvider(IServiceCollection services) : base(services) { }
        public AzureQueueServiceBuilder<T> WithAzure(Installation installation = Installation.Default)
          => AndWithAzure(installation);
        public AzureQueueServiceBuilder<T> AndWithAzure(Installation installation = Installation.Default)
          => new(installation, this);
        public IServiceCollection Configure()
        {
            ServiceCollection.AddSingleton(new Options<IQueueManager<T>>(Services));
            ServiceCollection.AddSingleton<IQueueManager<T>, QueueManager<T>>();
            ServiceCollection.AddRystemFullyAddedCallback(() => ServiceLocator.GetService<IQueueManager<T>>().WarmUpAsync());
            return ServiceCollection;
        }
        public RystemQueueServiceProvider ConfigureAfterBuild()
        {
            Configure();
            ServiceLocatorAtRuntime.Rebuild();
            return RystemQueueServiceProvider.Instance;
        }
    }
}