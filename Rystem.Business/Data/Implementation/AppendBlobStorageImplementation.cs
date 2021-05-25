using Rystem.Azure.Integration.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Rystem.Business.Data.Implementantion
{
    internal class AppendBlobStorageImplementation<TEntity> : IDataImplementation<TEntity>
        where TEntity : IData
    {
        private readonly Type EntityType;
        private readonly BlobStorageIntegration Integration;
        internal AppendBlobStorageImplementation(BlobStorageIntegration integration, Type entityType)
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
        public Task<bool> SetPropertiesAsync(TEntity entity, dynamic properties)
            => Integration.SetBlobPropertiesAsync(entity.Name, properties);
        public Task<bool> WriteAsync(TEntity entity, Stream stream, dynamic _)
            => Integration.WriteAppendAsync(entity.Name, stream);
    }
}