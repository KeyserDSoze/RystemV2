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
        private readonly IDictionary<Installation, IDocumentImplementation<TEntity>> Integrations = new Dictionary<Installation, IDocumentImplementation<TEntity>>();
        private readonly IDictionary<Installation, ProvidedService> DocumentConfiguration;
        private static readonly object TrafficLight = new();
        private IDocumentImplementation<TEntity> Integration(Installation installation)
        {
            if (!Integrations.ContainsKey(installation))
                lock (TrafficLight)
                    if (!Integrations.ContainsKey(installation))
                    {
                        ProvidedService configuration = DocumentConfiguration[installation];
                        switch (configuration.Type)
                        {
                            case ServiceProviderType.AzureTableStorage:
                                Integrations.Add(installation, new TableStorageImplementation<TEntity>(new TableStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]), DefaultEntity));
                                break;
                            case ServiceProviderType.AzureBlobStorage:
                                Integrations.Add(installation, new BlobStorageImplementation<TEntity>(new BlobStorageIntegration(configuration.Configurations, AzureManager.Instance.Storages[configuration.ServiceKey]), this.DefaultEntity));
                                break;
                            default:
                                throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
                        }
                    }
            return Integrations[installation];
        }
        private readonly Type DefaultEntity;
        public DocumentManager(RystemDocumentServiceProvider serviceProvider)
        {
            DocumentConfiguration = serviceProvider.Services.ToDictionary(x => x.Key, x => x.Value);
            DefaultEntity = serviceProvider.InstanceType;
        }
        public async Task<bool> DeleteAsync(TEntity entity, Installation installation)
            => await Integration(installation).DeleteAsync(entity).NoContext();
        public async Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entity, Installation installation)
             => await Integration(installation).DeleteBatchAsync(entity).NoContext();
        public async Task<bool> ExistsAsync(TEntity entity, Installation installation)
            => await Integration(installation).ExistsAsync(entity).NoContext();
        public async Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Installation installation, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default)
            => await Integration(installation).GetAsync(entity, expression, takeCount).NoContext();
        public async Task<bool> UpdateAsync(TEntity entity, Installation installation)
            => await Integration(installation).UpdateAsync(entity).NoContext();
        public async Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entity, Installation installation)
            => await Integration(installation).UpdateBatchAsync(entity).NoContext();
        public string GetName(Installation installation = Installation.Default)
            => Integration(installation).GetName();
    }
}
