using Rystem.Azure;
using Rystem.Business.Document.Implementantion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rystem.Business.Document
{
    internal class DocumentManager<TEntity> : IDocumentManager<TEntity>
        where TEntity : new()
    {
        private readonly IDictionary<Installation, IDocumentImplementation<TEntity>> Implementations = new Dictionary<Installation, IDocumentImplementation<TEntity>>();
        private readonly IDictionary<Installation, ProvidedService> DocumentConfiguration;
        private readonly object TrafficLight = new();
        private IDocumentImplementation<TEntity> Implementation(Installation installation)
        {
            if (!Implementations.ContainsKey(installation))
                lock (TrafficLight)
                    if (!Implementations.ContainsKey(installation))
                    {
                        ProvidedService configuration = DocumentConfiguration[installation];
                        switch (configuration.Type)
                        {
                            case ServiceProviderType.AzureTableStorage:
                                Implementations.Add(installation, new TableStorageImplementation<TEntity>(Manager.TableStorage(configuration.Configurations, configuration.ServiceKey), configuration.Options));
                                break;
                            case ServiceProviderType.AzureBlockBlobStorage:
                                Implementations.Add(installation, new BlobStorageImplementation<TEntity>(Manager.BlobStorage(configuration.Configurations, configuration.ServiceKey), configuration.Options));
                                break;
                            case ServiceProviderType.AzureCosmosNoSql:
                                Implementations.Add(installation, new CosmosNoSqlImplementation<TEntity>(Manager.CosmosNoSql(configuration.Configurations, configuration.ServiceKey), configuration.Options));
                                break;
                            default:
                                throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                        }
                    }
            return Implementations[installation];
        }
        private readonly AzureManager Manager;
        public DocumentManager(Options<IDocumentManager<TEntity>> options, AzureManager manager)
        {
            DocumentConfiguration = options.Services;
            Manager = manager;
        }
        private const string DateTimeStringForPrimaryKey = "yyyyMMdd";
        private static TEntity Normalize(TEntity entity, RystemDocumentServiceProviderOptions options)
        {
            if (options.PrimaryKey.GetValue(entity) == default)
                options.PrimaryKey.SetValue(entity, DateTime.UtcNow.ToString(DateTimeStringForPrimaryKey));
            if (options.SecondaryKey.GetValue(entity) == default)
                options.PrimaryKey.SetValue(entity, Alea.GetTimedKey());
            return entity;
        }
        public Task<bool> DeleteAsync(TEntity entity, Installation installation = Installation.Default)
            => Implementation(installation).DeleteAsync(entity);
        public Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entity, Installation installation = Installation.Default)
             => Implementation(installation).DeleteBatchAsync(entity);
        public Task<bool> ExistsAsync(TEntity entity, Installation installation = Installation.Default)
            => Implementation(installation).ExistsAsync(entity);
        public Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default, Installation installation = Installation.Default)
            => Implementation(installation).GetAsync(entity, expression, takeCount);
        public Task<bool> UpdateAsync(TEntity entity, Installation installation = Installation.Default)
        {
            var implementation = Implementation(installation);
            return implementation.UpdateAsync(Normalize(entity, implementation.Options));
        }

        public Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entity, Installation installation = Installation.Default)
        {
            var implementation = Implementation(installation);
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
            => Implementation(installation).GetName();
    }
}