using Rystem.Azure;
using Rystem.Azure.Integration.Cache;
using Rystem.Azure.Integration.Storage;
using Rystem.Business;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public interface ISequenceManager<T>
    {
        void Add(T entity, Installation installation);
        void Flush(Installation installation);
    }
    internal class SequenceManager<T> : ISequenceManager<T>
    {
        private readonly Dictionary<Installation, string> Implementations = new();
        private readonly Dictionary<Installation, ProvidedService> SequenceConfiguration;
        private bool MemoryIsActive { get; }
        private static readonly object TrafficLight = new();
        public string Implementation(Installation installation)
        {
            if (!Implementations.ContainsKey(installation))
                lock (TrafficLight)
                    if (!Implementations.ContainsKey(installation))
                    {
                        ProvidedService configuration = SequenceConfiguration[installation];
                        var property = (SequenceProperty<T>)configuration.Configurations;
                        switch (configuration.Type)
                        {
                            case ServiceProviderType.InMemory:
                                Sequence.Create(property, QueueType.FirstInFirstOut);
                                Implementations.Add(installation, property.Name);
                                break;
                            case ServiceProviderType.InMemory2:
                                Sequence.Create(property, QueueType.LastInFirstOut);
                                Implementations.Add(installation, property.Name);
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
            SequenceConfiguration = options.Services;
            Manager = manager;
        }

        public void Add(T entity, Installation installation)
            => Sequence.Enqueue(entity, Implementation(installation));
        public void Flush(Installation installation)
            => Sequence.Flush(Implementation(installation), true);
    }
}