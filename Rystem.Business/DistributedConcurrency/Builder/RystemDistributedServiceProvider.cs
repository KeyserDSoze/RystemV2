using Microsoft.Extensions.DependencyInjection;
using Rystem.Business;

namespace Rystem.Concurrency
{
    public sealed class RystemDistributedServiceProvider<T> : Business.ServiceProvider
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
            ServiceCollection.AddRystemFullyAddedCallback(() => ServiceLocator.GetService<IDistributedManager<T>>().WarmUpAsync());
            return ServiceCollection;
        }
    }
}