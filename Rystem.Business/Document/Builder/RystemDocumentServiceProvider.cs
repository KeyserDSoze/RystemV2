using Microsoft.Extensions.DependencyInjection;
using Rystem.Business.Document;

namespace Rystem.Business
{
    public class RystemDocumentServiceProvider<T> : ServiceProvider
        where T : new()
    {
        public RystemDocumentServiceProvider(IServiceCollection services) : base(services)
        {
        }
        public AzureDocumentServiceBuilder<T> WithAzure(Installation installation = Installation.Default)
          => AndWithAzure(installation);
        public AzureDocumentServiceBuilder<T> AndWithAzure(Installation installation = Installation.Default)
          => new(installation, this);
        public IServiceCollection Configure()
        {
            ServiceCollection.AddSingleton(new Options<IDocumentManager<T>>(Services));
            ServiceCollection.AddSingleton<IDocumentManager<T>, DocumentManager<T>>();
            return ServiceCollection;
        }
    }
}