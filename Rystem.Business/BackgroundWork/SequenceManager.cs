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
        private static readonly object TrafficLight = new();
        public string Implementation(Installation installation)
        {
            if (!Implementations.ContainsKey(installation))
                lock (TrafficLight)
                    if (!Implementations.ContainsKey(installation))
                    {
                        ProvidedService configuration = SequenceConfigurations[installation];
                        var property = (SequenceProperty<T>)configuration.Configurations;
                        var options = (RystemAggregationServiceProviderOptions)configuration.Options;
                        switch (configuration.Type)
                        {
                            case ServiceProviderType.InMemory:
                                if (options.IsFirstInFirstOut)
                                {
                                    Queue.Create(property, QueueType.FirstInFirstOut);
                                    Implementations.Add(installation, property.Name);
                                }
                                else
                                {
                                    Queue.Create(property, QueueType.LastInFirstOut);
                                    Implementations.Add(installation, property.Name);
                                }
                                break;
                            default:
                                throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                        }
                    }
            return Implementations[installation];
        }
        private readonly AzureManager Manager;
        public SequenceManager(Options<ISequenceManager<T>> options, AzureManager manager)
        {
            SequenceConfigurations = options.Services;
            Manager = manager;
        }

        public void Add(T entity, Installation installation)
            => Queue.Enqueue(entity, Implementation(installation));
        public void Flush(Installation installation)
            => Queue.Flush(Implementation(installation), true);

        public Task<bool> WarmUpAsync()
        {
            foreach (var configuration in SequenceConfigurations)
                _ = Implementation(configuration.Key);
            return Task.FromResult(true);
        }
    }
}