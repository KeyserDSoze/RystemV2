namespace Rystem.Concurrency
{
    public static class ConfigurableDistributedConcurrencyExtensions
    {
        public static RystemDistributedServiceProvider<T> StartConfiguration<T>(this T configurableDistributedService)
            where T : IConfigurableDistributedConcurrencyKey 
            => RystemDistributedServiceProvider
                    .Configure(configurableDistributedService);
    }
}