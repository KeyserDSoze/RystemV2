using Rystem.Azure;
using Rystem.Azure.Integration.Storage;
using Rystem.Business.Document.Implementantion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business.Document
{
    internal class DocumentManager<TEntity>
        where TEntity : IDocument, new()
    {
        private readonly IDictionary<Installation, IDocumentImplementation<TEntity>> Implementations = new Dictionary<Installation, IDocumentImplementation<TEntity>>();
        private readonly IDictionary<Installation, ProvidedService> DocumentConfiguration;
        private readonly object TrafficLight = new();
        private readonly RystemServices Services = new();
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
                                Implementations.Add(installation, new TableStorageImplementation<TEntity>(Services.Factory.TableStorage(configuration.Configurations, configuration.ServiceKey), DefaultEntity));
                                break;
                            case ServiceProviderType.AzureBlockBlobStorage:
                                Implementations.Add(installation, new BlobStorageImplementation<TEntity>(Services.Factory.BlobStorage(configuration.Configurations, configuration.ServiceKey), DefaultEntity));
                                break;
                            default:
                                throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                        }
                    }
            return Implementations[installation];
        }
        private readonly Type DefaultEntity;
        public DocumentManager(RystemDocumentServiceProvider serviceProvider)
        {
            DocumentConfiguration = serviceProvider.Services.ToDictionary(x => x.Key, x => x.Value);
            DefaultEntity = serviceProvider.InstanceType;
        }
        private const string DateTimeStringForPrimaryKey = "yyyyMMdd";
        private static TEntity Normalize(TEntity entity)
        {
            if (entity.PrimaryKey == null)
                entity.PrimaryKey = DateTime.UtcNow.ToString(DateTimeStringForPrimaryKey);
            if (entity.SecondaryKey == null)
                entity.SecondaryKey = Alea.GetTimedKey();
            return entity;
        }
        public async Task<bool> DeleteAsync(TEntity entity, Installation installation)
            => await Implementation(installation).DeleteAsync(entity).NoContext();
        public async Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entity, Installation installation)
             => await Implementation(installation).DeleteBatchAsync(entity).NoContext();
        public async Task<bool> ExistsAsync(TEntity entity, Installation installation)
            => await Implementation(installation).ExistsAsync(entity).NoContext();
        public async Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Installation installation, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default)
            => await Implementation(installation).GetAsync(entity, expression, takeCount).NoContext();
        public async Task<bool> UpdateAsync(TEntity entity, Installation installation)
            => await Implementation(installation).UpdateAsync(Normalize(entity)).NoContext();
        public async Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entity, Installation installation)
            => await Implementation(installation).UpdateBatchAsync(entity.Select(x => Normalize(x))).NoContext();
        public string GetName(Installation installation = Installation.Default)
            => Implementation(installation).GetName();
    }
}