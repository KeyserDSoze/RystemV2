using Microsoft.Extensions.DependencyInjection;
using Rystem.Business;

namespace Rystem.Background
{
    public sealed class RystemAggregationServiceProvider<T> : Business.ServiceProvider
    {
        public RystemAggregationServiceProvider(IServiceCollection services) : base(services) { }
        public AggregationServiceBuilder<T> With(Installation installation = Installation.Default)
          => AndWith(installation);
        public AggregationServiceBuilder<T> AndWith(Installation installation = Installation.Default)
          => new(installation, this);
        public IServiceCollection Configure()
        {
            ServiceCollection.AddSingleton(new Options<ISequenceManager<T>>(Services));
            ServiceCollection.AddSingleton<ISequenceManager<T>, SequenceManager<T>>();
            ServiceCollection.AddRystemFullyAddedCallback(() => ServiceLocator.GetService<ISequenceManager<T>>().WarmUpAsync());
            return ServiceCollection;
        }
    }
}