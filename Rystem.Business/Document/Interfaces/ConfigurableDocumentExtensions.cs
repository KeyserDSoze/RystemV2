namespace Rystem.Business
{
    public static class ConfigurableDocumentExtensions
    {
        public static RystemDocumentServiceProvider<T> StartConfiguration<T>(this T configurableDocument)
            where T : IConfigurableDocument
            => RystemDocumentServiceProvider
                    .Configure(configurableDocument);
    }
}