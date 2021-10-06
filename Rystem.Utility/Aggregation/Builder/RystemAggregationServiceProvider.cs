using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Background
{
    public sealed class RystemAggregationServiceProvider
    {
        internal static readonly RystemAggregationServiceProvider Instance = new();
        public static RystemAggregationServiceProvider<T> Configure<T>(T entity)
            => new(ServiceLocatorAtRuntime.PrepareToAddNewService());
    }
    public sealed class RystemAggregationServiceProvider<T> : ServiceProvider
    {
        internal RystemAggregationServiceProvider(IServiceCollection services) : base(services) { }
        public AggregationServiceBuilder<T> With(Installation installation = Installation.Default)
          => AndWith(installation);
        public AggregationServiceBuilder<T> AndWith(Installation installation = Installation.Default)
          => new(installation, this);
        public IServiceCollection Configure()
        {
            ServiceCollection.AddSingleton(new Options<IAggregationManager<T>>(Services));
            ServiceCollection.AddSingleton<IAggregationManager<T>, AggregationManager<T>>();
            ServiceCollection.AddRystemFullyAddedCallback(() => ServiceLocator.GetService<IAggregationManager<T>>().WarmUpAsync());
            return ServiceCollection;
        }
        public RystemAggregationServiceProvider ConfigureAfterBuild()
        {
            Configure();
            ServiceLocatorAtRuntime.Rebuild();
            return RystemAggregationServiceProvider.Instance;
        }
    }
}