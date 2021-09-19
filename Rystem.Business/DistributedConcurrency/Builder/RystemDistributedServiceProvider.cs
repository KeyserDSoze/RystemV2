using Microsoft.Extensions.DependencyInjection;
using Rystem.Business;
using System.Linq;

namespace Rystem.Concurrency
{
    public class RystemDistributedServiceProvider<T> : Business.ServiceProvider
        where T : IDistributedConcurrencyKey
    {
        public RystemDistributedServiceProvider(IServiceCollection services) : base(services)
        {
        }
        public AzureDistributedServiceBuilder<T> WithAzure(Installation installation = Installation.Default)
          => AndWithAzure(installation);
        public AzureDistributedServiceBuilder<T> AndWithAzure(Installation installation = Installation.Default)
          => new(installation, this);
        public IServiceCollection Configure()
        {
            ServiceCollection.AddSingleton(new Options<IDistributedManager<T>>(Services));
            ServiceCollection.AddSingleton<IDistributedManager<T>, DistributedManager<T>>();
            return ServiceCollection;
        }
    }
}