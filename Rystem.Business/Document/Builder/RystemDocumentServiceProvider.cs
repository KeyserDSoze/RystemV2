using Microsoft.Extensions.DependencyInjection;
using Rystem.Business.Document;

namespace Rystem.Business
{
    public sealed class RystemDocumentServiceProvider
    {
        private RystemDocumentServiceProvider() { }
        internal static readonly RystemDocumentServiceProvider Instance = new();
        public static RystemDocumentServiceProvider<T> Configure<T>(T entity)
            => new(ServiceLocatorAtRuntime.PrepareToAddNewService());
    }
    public sealed class RystemDocumentServiceProvider<T> : ServiceProvider
    {
        internal RystemDocumentServiceProvider(IServiceCollection services) : base(services)
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
            ServiceCollection.AddRystemFullyAddedCallback(() => ServiceLocator.GetService<IDocumentManager<T>>().WarmUpAsync());
            return ServiceCollection;
        }
        public RystemDocumentServiceProvider ConfigureAfterBuild()
        {
            Configure();
            ServiceLocatorAtRuntime.Rebuild();
            return RystemDocumentServiceProvider.Instance;
        }
    }
}