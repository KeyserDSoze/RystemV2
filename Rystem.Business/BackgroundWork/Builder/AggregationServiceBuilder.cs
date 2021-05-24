using Rystem.Business;

namespace Rystem.BackgroundWork
{
    public class AggregationServiceBuilder : ServiceBuilder<RystemAggregationServiceProvider>
    {
        public AggregationServiceBuilder(Installation installation, ServiceProvider<RystemAggregationServiceProvider> rystemServiceProvider) : base(installation, rystemServiceProvider)
        {

        }
        public RystemAggregationServiceProvider WithFirstInFirstOut<T>(SequenceProperty<T> configuration)
            => (RystemAggregationServiceProvider)WithIntegration(ServiceProviderType.InMemory, configuration, default);
        public RystemAggregationServiceProvider WithLastInFirstOut<T>(SequenceProperty<T> configuration)
            => (RystemAggregationServiceProvider)WithIntegration(ServiceProviderType.InMemory2, configuration, default);
    }
}