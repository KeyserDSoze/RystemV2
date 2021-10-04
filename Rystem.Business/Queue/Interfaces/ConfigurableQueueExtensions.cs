namespace Rystem.Business
{
    public static class ConfigurableQueueExtensions
    {
        public static RystemQueueServiceProvider<T> StartConfiguration<T>(this T configurableQueue)
            where T : IConfigurableQueue 
            => RystemQueueServiceProvider
                    .Configure(configurableQueue);
    }
}