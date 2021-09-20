﻿using Microsoft.Extensions.DependencyInjection;

namespace Rystem.Business
{
    public class RystemQueueServiceProvider<T> : ServiceProvider
    {
        public RystemQueueServiceProvider(IServiceCollection services) : base(services) { }
        public AzureQueueServiceBuilder<T> WithAzure(Installation installation = Installation.Default)
          => AndWithAzure(installation);
        public AzureQueueServiceBuilder<T> AndWithAzure(Installation installation = Installation.Default)
          => new(installation, this);
        public IServiceCollection Configure()
        {
            ServiceCollection.AddSingleton(new Options<IQueueManager<T>>(Services));
            ServiceCollection.AddSingleton<IQueueManager<T>, QueueManager<T>>();
            return ServiceCollection;
        }
    }
}