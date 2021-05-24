using Rystem.Business;

namespace Rystem.BackgroundWork
{
    public class RystemAggregationServiceProvider : ServiceProvider<RystemAggregationServiceProvider>
    {
        private RystemAggregationServiceProvider() { }
        public static AggregationServiceBuilder Configure(Installation installation = Installation.Default)
          => new RystemAggregationServiceProvider().AndConfigure(installation);
        public AggregationServiceBuilder AndConfigure(Installation installation = Installation.Default)
          => new(installation, this);
    }
}