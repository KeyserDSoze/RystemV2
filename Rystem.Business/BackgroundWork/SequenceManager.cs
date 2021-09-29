using Rystem.Azure;
using Rystem.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Background
{
    internal sealed class SequenceManager<T> : ISequenceManager<T>
    {
        private readonly Dictionary<Installation, string> Implementations = new();
        private readonly Dictionary<Installation, ProvidedService> SequenceConfigurations;
        private readonly AzureManager Manager;
        public SequenceManager(Options<ISequenceManager<T>> options, AzureManager manager)
        {
            SequenceConfigurations = options.Services;
            Manager = manager;
            foreach(var conf in SequenceConfigurations)
            {
                ProvidedService configuration = SequenceConfigurations[conf.Key];
                var property = (SequenceProperty<T>)configuration.Configurations;
                var aggregationOptions = (RystemAggregationServiceProviderOptions)configuration.Options;
                switch (configuration.Type)
                {
                    case ServiceProviderType.InMemory:
                        if (aggregationOptions.IsFirstInFirstOut)
                        {
                            Queue.Create(property, QueueType.FirstInFirstOut);
                            Implementations.Add(conf.Key, property.Name);
                        }
                        else
                        {
                            Queue.Create(property, QueueType.LastInFirstOut);
                            Implementations.Add(conf.Key, property.Name);
                        }
                        break;
                    default:
                        throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                }
            }
        }
        public void Add(T entity, Installation installation)
            => Queue.Enqueue(entity, Implementations[installation]);
        public void Flush(Installation installation)
            => Queue.Flush(Implementations[installation], true);
        public Task<bool> WarmUpAsync() 
            => Task.FromResult(true);
    }
}