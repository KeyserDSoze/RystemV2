﻿using Azure;
using Azure.Identity;
using Microsoft.Azure.Cosmos.Table;
using Rystem.Concurrency;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Azure.IntegrationWithAzure.Storage
{
    internal class TableStorageIntegration : BaseStorageClient
    {
        private CloudTable Context;
        private readonly string TableName;
        private readonly string RaceId = Guid.NewGuid().ToString("N");
        //public TableStorageIntegration(string tableName, string accountName) : base(accountName)
        //    => TableName = tableName;
        public TableStorageIntegration(string tableName, string accountName, string accountKey) : base(accountName, accountKey)
            => TableName = tableName;
        private async Task<CloudTable> GetContextAsync()
        {
            if (Context == null)
                await RaceCondition.RunAsync(async () =>
                {
                    if (Context == null)
                    {
                        var storageAccount = CloudStorageAccount.Parse(ConnectionString);
                        var client = storageAccount.CreateCloudTableClient();
                        var tableClient = client.GetTableReference(TableName);
                        if (!await tableClient.ExistsAsync().NoContext())
                            await tableClient.CreateIfNotExistsAsync().NoContext();
                        Context = tableClient;
                    }
                }, RaceId).NoContext();
            return Context;
        }
        public async Task<bool> DeleteAsync(DynamicTableEntity entity)
        {
            var client = Context ?? await GetContextAsync();
            TableOperation operation = TableOperation.Delete(entity);
            return (await client.ExecuteAsync(operation).NoContext()).HttpStatusCode == 204;
        }
        public async Task<DynamicTableEntity> GetAsync(DynamicTableEntity entity)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            TableOperation operation = TableOperation.Retrieve<DynamicTableEntity>(entity.PartitionKey, entity.RowKey);
            TableResult result = await client.ExecuteAsync(operation).NoContext();
            return result.Result == null ? default : (DynamicTableEntity)result.Result;
        }

        public async Task<List<DynamicTableEntity>> QueryAsync(string query = null, int? takeCount = null, TableContinuationToken token = null)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            List<DynamicTableEntity> items = new();
            do
            {
                TableQuerySegment<DynamicTableEntity> seg = await client.ExecuteQuerySegmentedAsync(new TableQuery<DynamicTableEntity>() { FilterString = query, TakeCount = takeCount }, token).NoContext();
                token = seg.ContinuationToken;
                items.AddRange(seg);
                if (takeCount != null && items.Count >= takeCount)
                    break;
            } while (token != null);
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
                TableBatchOperation batch = new TableBatchOperation();
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
    }
}