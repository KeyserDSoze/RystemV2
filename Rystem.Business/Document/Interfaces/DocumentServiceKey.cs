namespace Rystem.Business
{
    public abstract class DocumentServiceKey<T>
        where T : new()
    {
        private protected readonly RystemDocumentServiceProvider<T> RystemDocumentServiceProvider;
        private protected readonly RystemDocumentServiceProviderOptions Options;
        public DocumentServiceKey(ServiceProvider rystemServiceProvider, RystemDocumentServiceProviderOptions options)
        {
            RystemDocumentServiceProvider = (RystemDocumentServiceProvider<T>)rystemServiceProvider;
            Options = options;
        }
    }
}