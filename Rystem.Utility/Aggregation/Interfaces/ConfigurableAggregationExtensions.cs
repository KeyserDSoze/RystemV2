namespace Rystem.Background
{
    public static class ConfigurableAggregationExtensions
    {
        public static RystemAggregationServiceProvider<T> StartConfiguration<T>(this T configurableAggregator)
            where T : IConfigurableAggregation 
            => RystemAggregationServiceProvider
                    .Configure(configurableAggregator);
    }
}