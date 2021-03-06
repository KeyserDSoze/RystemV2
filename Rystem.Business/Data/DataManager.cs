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
        private readonly IDictionary<Installation, ProvidedService> DataConfigurations;
        private readonly AzureManager Manager;
        public DataManager(Options<IDataManager<TEntity>> options, AzureManager manager)
        {
            DataConfigurations = options.Services;
            Manager = manager;
            foreach (var conf in DataConfigurations)
            {
                ProvidedService configuration = DataConfigurations[conf.Key];
                switch (configuration.Type)
                {
                    case ServiceProviderType.AzureBlockBlobStorage:
                        Implementations.Add(conf.Key, new BlockBlobStorageImplementation<TEntity>(Manager.BlobStorage(configuration.Configurations, configuration.ServiceKey), configuration.Options));
                        break;
                    case ServiceProviderType.AzureAppendBlobStorage:
                        Implementations.Add(conf.Key, new AppendBlobStorageImplementation<TEntity>(Manager.BlobStorage(configuration.Configurations, configuration.ServiceKey), configuration.Options));
                        break;
                    default:
                        throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                }
            }
        }
        public Task<bool> DeleteAsync(string name, Installation installation = Installation.Default)
          => Implementations[installation].DeleteAsync(name);
        public Task<bool> ExistsAsync(string name, Installation installation = Installation.Default)
            => Implementations[installation].ExistsAsync(name);
        public async Task<TEntity> ReadAsync(string name, Installation installation = Installation.Default)
        {
            var value = await ReadStreamAsync(name, installation).NoContext();
            return await value.FromJsonAsync<TEntity>().NoContext();
        }
        public async Task<T> ReadAsync<T>(string name, Installation installation = Installation.Default)
        {
            var value = await ReadStreamAsync(name, installation).NoContext();
            return await value.FromJsonAsync<T>().NoContext();
        }
        public async Task<List<TEntity>> ReadMultipleAsync(string name, Installation installation = Installation.Default)
        {
            var value = await ReadStreamAsync(name, installation).NoContext();
            return await value.FromJsonAsync<List<TEntity>>().NoContext();
        }
        public async Task<Stream> ReadStreamAsync(string name, Installation installation = Installation.Default)
        {
            var value = await Implementations[installation].ReadAsync(name).NoContext();
            value.Position = 0;
            return value;
        }
        public Task<IEnumerable<(string Name, Stream Value)>> ListStreamAsync(string startsWith, int? takeCount = null, Installation installation = Installation.Default)
            => Implementations[installation].ListAsync(startsWith, takeCount);

        public async IAsyncEnumerable<(string Name, TEntity Content)> ListAsync(string startsWith, int? takeCount = null, Installation installation = Installation.Default)
        {
            var implementation = Implementations[installation];
            foreach (var (Name, Value) in await ListStreamAsync(startsWith, takeCount ?? int.MaxValue, installation).NoContext())
                if (!implementation.IsMultipleLines)
                    yield return (Name, await Value.FromJsonAsync<TEntity>().NoContext());
                else
                    foreach (var entity in (await Value.ConvertToStringAsync().NoContext()).Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.FromJson<TEntity>()))
                        yield return (Name, entity);
        }
        public Task<bool> WriteAsync(string name, Stream stream, dynamic options, Installation installation = Installation.Default)
            => Implementations[installation].WriteAsync(name, stream, options);
        public async Task<bool> WriteAsync(string name, string value, dynamic options, Installation installation = Installation.Default)
            => await WriteAsync(name, await value.ToStreamAsync().NoContext(), options, installation).NoContext();
        public Task<bool> WriteAsync(string name, byte[] value, dynamic options, Installation installation = Installation.Default)
            => WriteAsync(name, value.ToStream(), options, installation);
        private static readonly BlobUploadOptions JsonBlobUploadOptions = new()
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = "application/json"
            }
        };
        public async Task<bool> WriteAsync(string name, TEntity value, Installation installation = Installation.Default)
        {
            if (value is Stream stream)
            {
                return await WriteAsync(name, stream, null, installation).NoContext();
            }
            else
            {
                return await WriteAsync(name, await (Implementations[installation].IsMultipleLines ? $"{value.ToJson()}\n" : value.ToJson()).ToStreamAsync(),
                               JsonBlobUploadOptions, installation).NoContext();
            }
        }

        public async Task<bool> WriteAsync<T>(string name, T value, Installation installation = Installation.Default)
        {
            if (value is Stream stream)
            {
                return await WriteAsync(name, stream, null, installation).NoContext();
            }
            else
            {
                return await WriteAsync(name, await (Implementations[installation].IsMultipleLines ? $"{value.ToJson()}\n" : value.ToJson()).ToStreamAsync(),
               JsonBlobUploadOptions, installation).NoContext();
            }
        }
        public Task<bool> SetPropertiesAsync(string name, dynamic properties, Installation installation = Installation.Default)
            => Implementations[installation].SetPropertiesAsync(name, properties);
        public string GetName(TEntity entity, Installation installation = Installation.Default)
        {
            var implementation = Implementations[installation];
            return implementation.Options.Name.GetValue(entity)?.ToString();
        }
        public async Task<bool> WarmUpAsync()
        {
            List<Task> tasks = new();
            foreach (var configuration in DataConfigurations)
                tasks.Add(Implementations[configuration.Key].WarmUpAsync());
            await Task.WhenAll(tasks);
            return true;
        }
        public Task<List<(string Uri, string Name)>> SearchAsync(string startsWith, int? takeCount = null, Installation installation = Installation.Default)
            => Implementations[installation].SearchAsync(startsWith, takeCount);
    }
}