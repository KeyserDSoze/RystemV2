using Rystem.Azure.Integration.Storage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Business.Data.Implementantion
{
    internal class BlockBlobStorageImplementation<TEntity> : IDataImplementation<TEntity>
    {
        private readonly BlobStorageIntegration Integration;
        public bool IsMultipleLines { get; }
        public RystemDataServiceProviderOptions Options { get; }
        internal BlockBlobStorageImplementation(BlobStorageIntegration integration, RystemDataServiceProviderOptions options)
        {
            Integration = integration;
            Options = options;
            IsMultipleLines = false;
        }
        public Task<bool> DeleteAsync(string name)
            => Integration.DeleteAsync(name);
        public Task<bool> ExistsAsync(string name)
            => Integration.ExistsAsync(name);
        public async Task<Stream> ReadAsync(string name)
        {
            var value = await Integration.ReadAsync(name, false).NoContext();
            return value.Content;
        }
        public async Task<IEnumerable<(string Name, Stream Value)>> ListAsync(string filter, int? takeCount = null)
            => (await Integration.ListAsync(filter, takeCount).NoContext()).Select(x => (x.Name, x.Content));
        public Task<bool> WriteAsync(string name, Stream stream, dynamic options)
            => Integration.WriteBlockAsync(name, stream, options);
        public Task<bool> SetPropertiesAsync(string name, dynamic properties)
            => Integration.SetBlobPropertiesAsync(name, properties);
        public Task<bool> WarmUpAsync()
            => Integration.WarmUpAsync();
    }
}