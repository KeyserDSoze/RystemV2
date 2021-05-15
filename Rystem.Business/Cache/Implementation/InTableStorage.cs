using Rystem.Azure.Integration.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Business
{
    internal class InTableStorage<T> : ICacheImplementation<T>
    {
        private CloudTable context;
        private readonly RaceCondition RaceCondition = new RaceCondition();
        private async Task<CloudTable> GetContextAsync()
        {
            if (context == null)
                await RaceCondition.ExecuteAsync(async () =>
                {
                    if (context == null)
                    {
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Configuration.ConnectionString);
                        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                        var preContext = tableClient.GetTableReference(TableName);
                        if (!await preContext.ExistsAsync().NoContext())
                            await preContext.CreateIfNotExistsAsync().NoContext();
                        context = preContext;
                    }
                }).NoContext();
            return context;
        }
        private static long ExpireCache = 0;
        private const string TableName = "RystemCache";
        private readonly string FullName;
        private readonly CacheConfiguration Properties;
        private readonly RystemCacheConfiguration Configuration;
        internal InTableStorage(TableStorageIntegration configuration)
        {
            this.FullName = configuration.CloudProperties.Namespace;
            Configuration = configuration;
            Properties = configuration.CloudProperties;
            ExpireCache = Properties.ExpireTimeSpan.Ticks;
        }
        public async Task<T> InstanceAsync(string key)
        {
            var client = context ?? await GetContextAsync();
            TableOperation operation = TableOperation.Retrieve<RystemCache>(FullName, key);
            TableResult result = await client.ExecuteAsync(operation).NoContext();
            return result.Result != default ? ((RystemCache)result.Result).Data.FromDefaultJson<T>() : default;
        }
        public async Task<bool> UpdateAsync(string key, T value, TimeSpan expiringTime)
        {
            var client = context ?? await GetContextAsync();
            long expiring = ExpireCache;
            if (expiringTime != default)
                expiring = expiringTime.Ticks;
            RystemCache rystemCache = new RystemCache()
            {
                PartitionKey = FullName,
                RowKey = key,
                Data = value.ToDefaultJson(),
                E = expiring > 0 ? expiring + DateTime.UtcNow.Ticks : DateTime.MaxValue.Ticks
            };
            TableOperation operation = TableOperation.InsertOrReplace(rystemCache);
            TableResult result = await client.ExecuteAsync(operation).NoContext();
            return result.HttpStatusCode == 204;
        }
        public async Task<bool> DeleteAsync(string key)
        {
            var client = context ?? await GetContextAsync();
            RystemCache rystemCache = new RystemCache()
            {
                PartitionKey = FullName,
                RowKey = key,
                ETag = "*"
            };
            TableOperation operation = TableOperation.Delete(rystemCache);
            try
            {
                TableResult result = await client.ExecuteAsync(operation).NoContext();
                return result.HttpStatusCode == 204;
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
            var client = context ?? await GetContextAsync();
            TableOperation operation = TableOperation.Retrieve<RystemCache>(FullName, key);
            TableResult result = await client.ExecuteAsync(operation).NoContext();
            if (result.Result == null)
                return CacheStatus<T>.NotOk();
            RystemCache cached = (RystemCache)result.Result;
            if (DateTime.UtcNow.Ticks > cached.E)
            {
                await this.DeleteAsync(key).NoContext();
                return CacheStatus<T>.NotOk();
            }
            return CacheStatus<T>.Ok(cached.Data.FromDefaultJson<T>());
        }
        public async Task<IEnumerable<string>> ListAsync()
        {
            var client = context ?? await GetContextAsync();
            TableQuery tableQuery = new TableQuery
            {
                FilterString = $"PartitionKey eq '{FullName}'"
            };
            TableContinuationToken tableContinuationToken = new TableContinuationToken();
            List<string> keys = new List<string>();
            do
            {
                TableQuerySegment<DynamicTableEntity> tableQuerySegment = await client.ExecuteQuerySegmentedAsync(tableQuery, tableContinuationToken).NoContext();
                IEnumerable<string> keysFromQuery = tableQuerySegment.Results.Select(x => x.RowKey);
                tableContinuationToken = tableQuerySegment.ContinuationToken;
                keys.AddRange(keysFromQuery);
            } while (tableContinuationToken != null);
            return keys;
        }
        public Task WarmUp()
            => Task.CompletedTask;
        private class RystemCache : TableEntity
        {
            public string Data { get; set; }
            public long E { get; set; }
        }
    }
}
