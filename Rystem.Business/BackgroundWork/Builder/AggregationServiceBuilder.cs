using Rystem.Business;

namespace Rystem.Background
{
    public sealed class AggregationServiceBuilder<T> : ServiceBuilder
    {
        public AggregationServiceBuilder(Installation installation, ServiceProvider rystemServiceProvider) : base(installation, rystemServiceProvider)
        {

        }
        public RystemAggregationServiceProvider<T> WithFirstInFirstOut(SequenceProperty<T> configuration)
            => (RystemAggregationServiceProvider<T>)WithIntegration<T, SequenceProperty<T>>(ServiceProviderType.InMemory, configuration, string.Empty, new RystemAggregationServiceProviderOptions(true));
        public RystemAggregationServiceProvider<T> WithLastInFirstOut(SequenceProperty<T> configuration)
            => (RystemAggregationServiceProvider<T>)WithIntegration<T, SequenceProperty<T>>(ServiceProviderType.InMemory, configuration, string.Empty, new RystemAggregationServiceProviderOptions(true));
    }
}