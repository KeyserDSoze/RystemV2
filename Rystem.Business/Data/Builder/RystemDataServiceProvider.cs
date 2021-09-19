using Microsoft.Extensions.DependencyInjection;
using Rystem.Business.Data;
using System.Linq;

namespace Rystem.Business
{
    public class RystemDataServiceProvider<T> : ServiceProvider
    {
        public RystemDataServiceProvider(IServiceCollection services) : base(services)
        {
        }
        public AzureDataServiceBuilder<T> WithAzure(Installation installation = Installation.Default)
          => AndWithAzure(installation);
        public AzureDataServiceBuilder<T> AndWithAzure(Installation installation = Installation.Default)
          => new(installation, this);
        public IServiceCollection Configure()
        {
            ServiceCollection.AddSingleton(new Options<IDataManager<T>>(Services));
            ServiceCollection.AddSingleton<IDataManager<T>, DataManager<T>>();
            return ServiceCollection;
        }
    }
}