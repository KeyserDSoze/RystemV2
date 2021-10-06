namespace Rystem.Business
{
    public static class ConfigurableDataExtensions
    {
        public static RystemDataServiceProvider<T> StartConfiguration<T>(this T configurableData)
            where T : IConfigurableData
            => RystemDataServiceProvider
                    .Configure(configurableData);
    }
}