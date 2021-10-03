using Rystem.Azure;
using Rystem.Azure.Integration.Cosmos;
using Rystem.Azure.Integration.Storage;
using Rystem.Business.Document.Implementantion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rystem.Business.Document
{
    internal sealed class DocumentManager<TEntity> : IDocumentManager<TEntity>
    {
        private readonly IDictionary<Installation, IDocumentImplementation<TEntity>> Implementations = new Dictionary<Installation, IDocumentImplementation<TEntity>>();
        private readonly IDictionary<Installation, ProvidedService> DocumentConfigurations;
        private readonly AzureManager Manager;
        public DocumentManager(Options<IDocumentManager<TEntity>> options, AzureManager manager)
        {
            DocumentConfigurations = options.Services;
            Manager = manager;
            foreach(var conf in DocumentConfigurations)
            {
                ProvidedService configuration = DocumentConfigurations[conf.Key];
                RystemDocumentServiceProviderOptions configuratedOptions = configuration.Options as RystemDocumentServiceProviderOptions;
                switch (configuration.Type)
                {
                    case ServiceProviderType.AzureTableStorage:
                        Implementations.Add(conf.Key, new TableStorageImplementation<TEntity>(Manager.TableStorage(configuration.Configurations as TableStorageConfiguration, configuration.ServiceKey), configuratedOptions));
                        break;
                    case ServiceProviderType.AzureBlockBlobStorage:
                        Implementations.Add(conf.Key, new BlobStorageImplementation<TEntity>(Manager.BlobStorage(configuration.Configurations as BlobStorageConfiguration, configuration.ServiceKey), configuratedOptions));
                        break;
                    case ServiceProviderType.AzureCosmosNoSql:
                        Implementations.Add(conf.Key, new CosmosNoSqlImplementation<TEntity>(Manager.CosmosNoSql(configuration.Configurations as CosmosConfiguration, configuration.ServiceKey), configuratedOptions));
                        break;
                    default:
                        throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                }
            }
        }
        private const string DateTimeStringForPrimaryKey = "yyyyMMdd";
        private static TEntity Normalize(TEntity entity, RystemDocumentServiceProviderOptions options)
        {
            if (options.PrimaryKey.GetValue(entity) == default)
                options.PrimaryKey.SetValue(entity, DateTime.UtcNow.ToString(DateTimeStringForPrimaryKey));
            if (options.SecondaryKey.GetValue(entity) == default)
                options.SecondaryKey.SetValue(entity, Alea.GetTimedKey());
            return entity;
        }
        public Task<bool> DeleteAsync(TEntity entity, Installation installation = Installation.Default)
            => Implementations[installation].DeleteAsync(entity);
        public Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entity, Installation installation = Installation.Default)
             => Implementations[installation].DeleteBatchAsync(entity);
        public Task<bool> ExistsAsync(TEntity entity, Installation installation = Installation.Default)
            => Implementations[installation].ExistsAsync(entity);
        public Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default, Installation installation = Installation.Default)
            => Implementations[installation].GetAsync(entity, expression, takeCount);
        public Task<bool> UpdateAsync(TEntity entity, Installation installation = Installation.Default)
        {
            var implementation = Implementations[installation];
            return implementation.UpdateAsync(Normalize(entity, implementation.Options));
        }
        public Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entity, Installation installation = Installation.Default)
        {
            var implementation = Implementations[installation];
            return implementation.UpdateBatchAsync(entity.Select(x => Normalize(x, implementation.Options)));
        }
        public async Task<TEntity> FirstOrDefaultAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default)
           => (await GetAsync(entity, expression, 1, installation).NoContext()).FirstOrDefault();
        public Task<IEnumerable<TEntity>> ListAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default)
           => GetAsync(entity, expression, null, installation);
        public Task<IEnumerable<TEntity>> TakeAsync(TEntity entity, int takeCount, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default)
           => GetAsync(entity, expression, takeCount, installation);
        public async Task<int> CountAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default)
          => (await GetAsync(entity, expression, null, installation).NoContext()).Count();
        public string GetName(Installation installation = Installation.Default)
            => Implementations[installation].GetName();

        public async Task<bool> WarmUpAsync()
        {
            List<Task> tasks = new();
            foreach (var configuration in DocumentConfigurations)
                tasks.Add(Implementations[configuration.Key].WarmUpAsync());
            await Task.WhenAll(tasks).NoContext();
            return true;
        }
    }
}