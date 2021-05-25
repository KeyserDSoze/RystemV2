using Azure.Storage.Blobs.Models;
using Rystem.Azure.Installation;
using Rystem.Azure.Integration.Storage;
using Rystem.Business.Data.Implementantion;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Business.Data
{
    internal sealed class DataManager<TEntity>
        where TEntity : IData
    {
        private readonly IDictionary<Installation, IDataImplementation<TEntity>> Implementations = new Dictionary<Installation, IDataImplementation<TEntity>>();
        private readonly IDictionary<Installation, ProvidedService> DataConfiguration;
        private static readonly object TrafficLight = new();
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
                                Implementations.Add(installation, new BlockBlobStorageImplementation<TEntity>(new BlobStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]), DefaultEntity));
                                break;
                            case ServiceProviderType.AzureAppendBlobStorage:
                                Implementations.Add(installation, new AppendBlobStorageImplementation<TEntity>(new BlobStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]), this.DefaultEntity));
                                break;
                            default:
                                throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                        }
                    }
            return Implementations[installation];
        }
        private readonly Type DefaultEntity;
        public DataManager(RystemDataServiceProvider serviceProvider)
        {
            DataConfiguration = serviceProvider.Services.ToDictionary(x => x.Key, x => x.Value);
            DefaultEntity = serviceProvider.InstanceType;
        }
        public Task<bool> DeleteAsync(TEntity entity, Installation installation)
          => Implementation(installation).DeleteAsync(entity);
        public Task<bool> ExistsAsync(TEntity entity, Installation installation)
            => Implementation(installation).ExistsAsync(entity);
        public async Task<T> ReadAsync<T>(TEntity entity, Installation installation)
        {
            var value = await ReadAsync(entity, installation);
            return await value.FromJsonAsync<T>();
        }
        public async Task<IEnumerable<T>> ReadAsync<T>(TEntity entity, bool breakLine = false, Installation installation = Installation.Default)
        {
            var value = await ReadAsync(entity, installation);
            if (!breakLine)
                return await value.FromJsonAsync<IEnumerable<T>>();
            else
            {
                List<T> values = new();
                foreach (var line in (await value.ConvertToStringAsync()).Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)))
                    values.Add(line.FromJson<T>());
                return values;
            }
        }
        public async Task<Stream> ReadAsync(TEntity entity, Installation installation)
        {
            var value = await Implementation(installation).ReadAsync(entity).NoContext();
            value.Position = 0;
            return value;
        }
        public Task<bool> WriteAsync(TEntity entity, Stream stream, dynamic options, Installation installation)
            => Implementation(installation).WriteAsync(entity, stream, options);
        public async Task<bool> WriteAsync(TEntity entity, string value, dynamic options, Installation installation)
            => await WriteAsync(entity, await value.ToStreamAsync(), options, installation).NoContext();
        public Task<bool> WriteAsync(TEntity entity, byte[] value, dynamic options, Installation installation)
            => WriteAsync(entity, value.ToStream(), options, installation);
        private const string ApplicationJson = "application/json";
        public async Task<bool> WriteAsync(TEntity entity, object value, bool breakLine, Installation installation)
            => await WriteAsync(entity, await (breakLine ? $"{value.ToJson()}\n" : value.ToJson()).ToStreamAsync(),
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = ApplicationJson
                    }
                }, installation).NoContext();
        public Task<bool> SetPropertiesAsync(TEntity entity, dynamic properties, Installation installation)
            => Implementation(installation).SetPropertiesAsync(entity, properties);
    }
}