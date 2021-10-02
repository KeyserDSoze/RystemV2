using Microsoft.Azure.Cosmos.Table;
using Rystem.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Azure.Integration.Storage
{
    public sealed class TableStorageIntegration : BaseStorageClient, IWarmUp
    {
        private CloudTable Context;
        private readonly string RaceId = Guid.NewGuid().ToString("N");
        public TableStorageConfiguration Configuration { get; }
        public TableStorageIntegration(TableStorageConfiguration configuration, StorageAccount options) : base(options)
            => Configuration = configuration;
        private async Task<CloudTable> GetContextAsync()
        {
            if (Context == default)
                await RaceCondition.RunAsync(async () =>
                {
                    if (Context == default)
                    {
                        var storageAccount = CloudStorageAccount.Parse(await (Account as IRystemOptions).GetConnectionStringAsync().NoContext());
                        var client = Account.TableClientConfiguration != default ?
                                storageAccount.CreateCloudTableClient(Account.TableClientConfiguration) : storageAccount.CreateCloudTableClient();
                        var tableClient = client.GetTableReference(Configuration.Name);
                        if (!await tableClient.ExistsAsync().NoContext())
                            await tableClient.CreateIfNotExistsAsync().NoContext();
                        Context = tableClient;
                    }
                }, RaceId).NoContext();
            return Context;
        }
        private const string Asterisk = "*";
        public async Task<bool> DeleteAsync(DynamicTableEntity entity)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            if (string.IsNullOrWhiteSpace(entity.ETag))
                entity.ETag = Asterisk;
            TableOperation operation = TableOperation.Delete(entity);
            return (await client.ExecuteAsync(operation).NoContext()).HttpStatusCode == 204;
        }
        public async Task<DynamicTableEntity> GetAsync(DynamicTableEntity entity)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            TableOperation operation = TableOperation.Retrieve<DynamicTableEntity>(entity.PartitionKey, entity.RowKey);
            TableResult result = await client.ExecuteAsync(operation).NoContext();
            return result.Result == default ? default : (DynamicTableEntity)result.Result;
        }

        public async Task<List<DynamicTableEntity>> QueryAsync(string query = default, int? takeCount = default, TableContinuationToken token = default)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            List<DynamicTableEntity> items = new();
            do
            {
                TableQuerySegment<DynamicTableEntity> seg = await client.ExecuteQuerySegmentedAsync(new TableQuery<DynamicTableEntity>() { FilterString = query, TakeCount = takeCount }, token).NoContext();
                token = seg.ContinuationToken;
                items.AddRange(seg);
                if (takeCount != default && items.Count >= takeCount)
                    break;
            } while (token != default);
            return items;
        }
        public async Task<bool> UpdateAsync(DynamicTableEntity entity)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            TableOperation operation = TableOperation.InsertOrReplace(entity);
            return (await client.ExecuteAsync(operation).NoContext()).HttpStatusCode == 204;
        }
        public async Task<bool> UpdateBatchAsync(IEnumerable<DynamicTableEntity> entities)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            bool result = true;
            foreach (var groupedEntity in entities.GroupBy(x => x.PartitionKey))
            {
                TableBatchOperation batch = new();
                foreach (DynamicTableEntity entity in groupedEntity)
                {
                    batch.InsertOrReplace(entity);
                    if (batch.Count == 100)
                    {
                        IList<TableResult> results = await client.ExecuteBatchAsync(batch).NoContext();
                        result &= results.All(x => x.HttpStatusCode == 204);
                        batch = new TableBatchOperation();
                    }
                }
                if (batch.Count > 0)
                    result &= (await client.ExecuteBatchAsync(batch).NoContext()).All(x => x.HttpStatusCode == 204);
            }
            return result;
        }
        public async Task<bool> DeleteBatchAsync(IEnumerable<DynamicTableEntity> entities)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            bool result = true;
            foreach (var groupedEntity in entities.GroupBy(x => x.PartitionKey))
            {
                TableBatchOperation batch = new();
                foreach (DynamicTableEntity entity in groupedEntity)
                {
                    batch.Delete(entity);
                    if (batch.Count == 100)
                    {
                        IList<TableResult> results = await client.ExecuteBatchAsync(batch).NoContext();
                        result &= (results.All(x => x.HttpStatusCode == 204));
                        batch = new TableBatchOperation();
                    }
                }
                if (batch.Count > 0)
                    result &= (await client.ExecuteBatchAsync(batch).NoContext()).All(x => x.HttpStatusCode == 204);
            }
            return result;
        }

        public async Task<bool> WarmUpAsync()
        {
            await GetContextAsync().NoContext();
            return true;
        }
    }
}