using Microsoft.Azure.Cosmos.Table;
using Rystem.Azure.Integration.Storage;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Business
{
    internal class InTableStorage<T> : ICacheImplementation<T>
    {
        private readonly TableStorageIntegration Integration;
        private readonly string Prefix;
        internal InTableStorage(TableStorageIntegration integration, string prefix)
        {
            Integration = integration;
            Prefix = prefix;
        }

        private const string ValueColumn = "Value";
        private const string ExpiringColumn = "Expire";
        public async Task<T> InstanceAsync(string key)
        {
            var instance = await Integration.GetAsync(new DynamicTableEntity()
            {
                PartitionKey = Prefix,
                RowKey = key
            }).NoContext();
            if (DateTime.UtcNow > instance.Properties[ExpiringColumn].DateTime)
                return default;
            else
                return instance.Properties[ValueColumn].StringValue.FromJson<T>();
        }
        public async Task<bool> UpdateAsync(string key, T value, TimeSpan expiringTime)
            => await Integration.UpdateAsync(new DynamicTableEntity
            {
                PartitionKey = Prefix,
                RowKey = key,
                Properties = new Dictionary<string, EntityProperty>()
                {
                    { ValueColumn, new EntityProperty(value.ToJson()) },
                    { ExpiringColumn, new EntityProperty(expiringTime == default ? DateTime.MaxValue : DateTime.UtcNow.Add(expiringTime)) }
                }
            }).NoContext();
        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                return await Integration.DeleteAsync(new DynamicTableEntity
                {
                    PartitionKey = Prefix,
                    RowKey = key
                }).NoContext();
            }
            catch (StorageException er)
            {
                if (er.HResult == -2146233088)
                    return true;
                throw er;
            }
        }
        public async Task<CacheStatus<T>> ExistsAsync(string key)
        {
            var instance = await InstanceAsync(key).NoContext();
            return instance != null ? CacheStatus<T>.Ok(instance) : CacheStatus<T>.NotOk();
        }
        public async Task<IEnumerable<string>> ListAsync()
            => (await Integration.QueryAsync($"PartitionKey eq '{Prefix}'").NoContext()).Select(x => x.RowKey);
        public Task<bool> WarmUpAsync()
            => Integration.WarmUpAsync();
    }
}