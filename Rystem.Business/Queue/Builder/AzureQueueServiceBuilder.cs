using Rystem.Azure.Integration.Message;
using Rystem.Azure.Integration.Storage;

namespace Rystem.Business
{
    public class AzureQueueServiceBuilder : ServiceBuilder<RystemQueueServiceProvider>
    {
        public AzureQueueServiceBuilder(Installation installation, ServiceProvider<RystemQueueServiceProvider> rystemServiceProvider) : base(installation, rystemServiceProvider)
        {
        }
        public RystemQueueServiceProvider WithQueueStorage(QueueStorageConfiguration configuration = default, string serviceKey = default)
            => (RystemQueueServiceProvider)WithIntegration(ServiceProviderType.AzureQueueStorage, configuration, serviceKey);
        public RystemQueueServiceProvider WithEventHub(EventHubConfiguration configuration = default, string serviceKey = default)
            => (RystemQueueServiceProvider)WithIntegration(ServiceProviderType.AzureEventHub, configuration, serviceKey);
        public RystemQueueServiceProvider WithServiceBus(ServiceBusConfiguration configuration = default, string serviceKey = default)
            => (RystemQueueServiceProvider)WithIntegration(ServiceProviderType.AzureServiceBus, configuration, serviceKey);
    }
}
