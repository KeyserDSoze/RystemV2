using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Storage;

namespace Rystem.Business
{
    public class AzureQueueServiceBuilder<T> : ServiceBuilder
    {
        public AzureQueueServiceBuilder(Installation installation, ServiceProvider rystemServiceProvider) : base(installation, rystemServiceProvider)
        {
        }
        public RystemQueueServiceProvider<T> WithQueueStorage(QueueStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemQueueServiceProvider<T>)WithIntegration(ServiceProviderType.AzureQueueStorage, configuration, serviceKey);
        public RystemQueueServiceProvider<T> WithEventHub(EventHubConfiguration configuration = default, string serviceKey = default)
            => (RystemQueueServiceProvider<T>)WithIntegration(ServiceProviderType.AzureEventHub, configuration, serviceKey);
        public RystemQueueServiceProvider<T> WithServiceBus(ServiceBusConfiguration configuration = default, string serviceKey = default)
            => (RystemQueueServiceProvider<T>)WithIntegration(ServiceProviderType.AzureServiceBus, configuration, serviceKey);
    }
}
