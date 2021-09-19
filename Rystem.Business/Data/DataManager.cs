using Azure.Storage.Blobs.Models;
using Rystem.Azure;
using Rystem.Business.Data.Implementantion;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Business.Data
{
    internal sealed class DataManager<TEntity> : IDataManager<TEntity>
    {
        private readonly IDictionary<Installation, IDataImplementation<TEntity>> Implementations = new Dictionary<Installation, IDataImplementation<TEntity>>();
        private readonly IDictionary<Installation, ProvidedService> DataConfiguration;
        private readonly object TrafficLight = new();
        private IDataImplementation<TEntity> Implementation(Installation installation)
        {
            if (!Implementations.ContainsKey(installation))
                lock (TrafficLight)
                    if (!Implementations.ContainsKey(installation))
                    {
                        ProvidedService configuration = DataConfiguration[installation];
                        switch (configuration.Type)
                        {
                            case ServiceProviderType.AzureBlockBlobStorage:
                                Implementations.Add(installation, new BlockBlobStorageImplementation<TEntity>(Manager.BlobStorage(configuration.Configurations, configuration.ServiceKey), configuration.Options));
                                break;
                            case ServiceProviderType.AzureAppendBlobStorage:
                                Implementations.Add(installation, new AppendBlobStorageImplementation<TEntity>(Manager.BlobStorage(configuration.Configurations, configuration.ServiceKey), configuration.Options));
                                break;
                            default:
                                throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                        }
                    }
            return Implementations[installation];
        }
        private readonly AzureManager Manager;
        public DataManager(Options<DataManager<TEntity>> options, AzureManager manager)
        {
            DataConfiguration = options.Services;
            Manager = manager;
        }
        public Task<bool> DeleteAsync(string name, Installation installation = Installation.Default)
          => Implementation(installation).DeleteAsync(name);
        public Task<bool> ExistsAsync(string name, Installation installation = Installation.Default)
            => Implementation(installation).ExistsAsync(name);
        public async Task<TEntity> ReadAsync(string name, Installation installation = Installation.Default)
        {
            var value = await ReadStreamAsync(name, installation).NoContext();
            return await value.FromJsonAsync<TEntity>().NoContext();
        }
        public async Task<List<TEntity>> ReadMultipleAsync(string name, Installation installation = Installation.Default)
        {
            var value = await ReadStreamAsync(name, installation).NoContext();
            return await value.FromJsonAsync<List<TEntity>>().NoContext();
        }
        public async Task<Stream> ReadStreamAsync(string name, Installation installation = Installation.Default)
        {
            var value = await Implementation(installation).ReadAsync(name).NoContext();
            value.Position = 0;
            return value;
        }
        public Task<IEnumerable<(string Name, Stream Value)>> ListStreamAsync(string startsWith, int? takeCount = null, Installation installation = Installation.Default)
            => Implementation(installation).ListAsync(startsWith, takeCount);

        public async IAsyncEnumerable<(string Name, List<TEntity> Content)> ListAsync(string startsWith, int? takeCount = null, Installation installation = Installation.Default)
        {
            var implementation = Implementation(installation);
            foreach (var (Name, Value) in await ListStreamAsync(startsWith, takeCount ?? int.MaxValue, installation).NoContext())
                if (!implementation.IsMultipleLines)
                    yield return (Name, new List<TEntity>() { await Value.FromJsonAsync<TEntity>().NoContext() });
                else
                    yield return (Name, (await Value.ConvertToStringAsync().NoContext()).Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.FromJson<TEntity>()).ToList());
        }
        public Task<bool> WriteAsync(string name, Stream stream, dynamic options, Installation installation = Installation.Default)
            => Implementation(installation).WriteAsync(name, stream, options);
        public async Task<bool> WriteAsync(string name, string value, dynamic options, Installation installation = Installation.Default)
            => await WriteAsync(name, await value.ToStreamAsync().NoContext(), options, installation).NoContext();
        public Task<bool> WriteAsync(string name, byte[] value, dynamic options, Installation installation = Installation.Default)
            => WriteAsync(name, value.ToStream(), options, installation);
        private const string ApplicationJson = "application/json";
        public async Task<bool> WriteAsync(string name, TEntity value, Installation installation = Installation.Default)
            => await WriteAsync(name, await (Implementation(installation).IsMultipleLines ? $"{value.ToJson()}\n" : value.ToJson()).ToStreamAsync(),
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = ApplicationJson
                    }
                }, installation).NoContext();
        public Task<bool> SetPropertiesAsync(string name, dynamic properties, Installation installation = Installation.Default)
            => Implementation(installation).SetPropertiesAsync(name, properties);
        public string GetName(TEntity entity, Installation installation = Installation.Default)
        {
            var implementation = Implementation(installation);
            return implementation.Options.Name.GetValue(entity).ToString();
        }
    }
}