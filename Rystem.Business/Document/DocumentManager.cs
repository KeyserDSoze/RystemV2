using Rystem.Azure.Installation;
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
        where TEntity : IDocument
    {
        private readonly IDictionary<Installation, IDocumentImplementation<TEntity>> Implementations = new Dictionary<Installation, IDocumentImplementation<TEntity>>();
        private readonly IDictionary<Installation, ProvidedService> DocumentConfiguration;
        private static readonly object TrafficLight = new();
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
                                Implementations.Add(installation, new TableStorageImplementation<TEntity>(new TableStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]), DefaultEntity));
                                break;
                            case ServiceProviderType.AzureBlobStorage:
                                Implementations.Add(installation, new BlobStorageImplementation<TEntity>(new BlobStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]), this.DefaultEntity));
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
        public async Task<bool> DeleteAsync(TEntity entity, Installation installation)
            => await Implementation(installation).DeleteAsync(entity).NoContext();
        public async Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entity, Installation installation)
             => await Implementation(installation).DeleteBatchAsync(entity).NoContext();
        public async Task<bool> ExistsAsync(TEntity entity, Installation installation)
            => await Implementation(installation).ExistsAsync(entity).NoContext();
        public async Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Installation installation, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default)
            => await Implementation(installation).GetAsync(entity, expression, takeCount).NoContext();
        public async Task<bool> UpdateAsync(TEntity entity, Installation installation)
            => await Implementation(installation).UpdateAsync(entity).NoContext();
        public async Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entity, Installation installation)
            => await Implementation(installation).UpdateBatchAsync(entity).NoContext();
        public string GetName(Installation installation = Installation.Default)
            => Implementation(installation).GetName();
    }
}
