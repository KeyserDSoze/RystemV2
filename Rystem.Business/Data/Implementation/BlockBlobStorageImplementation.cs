using Microsoft.Azure.Cosmos.Table;
using Rystem.Azure.Integration;
using Rystem.Azure.Integration.Storage;
using Rystem.Text;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Rystem.Business.Data.Implementantion
{
    internal class BlockBlobStorageImplementation<TEntity> : IDataImplementation<TEntity>
        where TEntity : IData
    {
        private readonly Type EntityType;
        private readonly BlobStorageIntegration Integration;
        internal BlockBlobStorageImplementation(BlobStorageIntegration integration, Type entityType)
        {
            Integration = integration;
            this.EntityType = entityType;
        }
        public Task<bool> DeleteAsync(TEntity entity)
            => Integration.DeleteAsync(entity.Name);
        public Task<bool> ExistsAsync(TEntity entity)
            => Integration.ExistsAsync(entity.Name);
        public async Task<Stream> ReadAsync(TEntity entity)
        {
            var value = await Integration.ReadAsync(entity.Name, false).NoContext();
            return value.Content;
        }
        public Task<bool> WriteAsync(TEntity entity, Stream stream, dynamic options) 
            => Integration.WriteBlockAsync(entity.Name, stream, options);
        public Task<bool> SetPropertiesAsync(TEntity entity, dynamic properties)
            => Integration.SetBlobPropertiesAsync(entity.Name, properties);
    }
}