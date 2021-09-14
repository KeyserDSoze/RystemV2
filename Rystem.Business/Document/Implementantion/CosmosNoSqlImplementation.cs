using Microsoft.Azure.Cosmos;
using Rystem.Azure.Integration.Cosmos;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Rystem.Business.Document.Implementantion
{
    internal class CosmosNoSqlImplementation<TEntity> : IDocumentImplementation<TEntity>
        where TEntity : IDocument, new()
    {
        private readonly Type EntityType;
        private readonly CosmosNoSqlIntegration Integration;
        private readonly List<PropertyInfo> Properties = new();
        private readonly string PrimaryKeyName = "PrimaryKey";
        private static readonly Type NoDocumentAttribute = typeof(NoDocumentAttribute);
        public CosmosNoSqlImplementation(CosmosNoSqlIntegration integration, Type entityType)
        {
            Integration = integration;
            EntityType = entityType;
            foreach (PropertyInfo pi in EntityType.GetProperties())
            {
                if (pi.GetCustomAttribute(NoDocumentAttribute) != default || pi.Name == DocumentImplementationConst.PrimaryKey || pi.Name == DocumentImplementationConst.SecondaryKey || pi.Name == DocumentImplementationConst.Timestamp)
                    continue;
                else
                    Properties.Add(pi);
            }
            if (Integration.Configuration.ContainerProperties?.PartitionKeyPath != default)
                PrimaryKeyName = Integration.Configuration.ContainerProperties.PartitionKeyPath.Trim('/');
        }

        public Task<bool> DeleteAsync(TEntity entity)
            => Integration.DeleteAsync<TEntity>(entity.SecondaryKey, new PartitionKey(entity.PrimaryKey), default);

        public Task<bool> ExistsAsync(TEntity entity)
            => Integration.ExistAsync<TEntity>(entity.SecondaryKey, new PartitionKey(entity.PrimaryKey), default);

        public async Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default)
        {
            List<TEntity> entities = new();
            await foreach (var value in Integration.ReadAsync(expression, takeCount,
                entity.PrimaryKey != default ? new QueryRequestOptions { PartitionKey = new PartitionKey(entity.PrimaryKey) } : default,
                default, default))
            {
                entities.Add(value);
            }
            return entities;
        }
        public Task<bool> UpdateAsync(TEntity entity)
        {
            return Integration.UpdateAsync(new PartitionKey(entity.PrimaryKey), GetCosmosItem(entity), null);
        }

        public async Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entities)
        {
            List<Task> tasks = new();
            foreach (var entitiesByPartitionKey in entities.GroupBy(x => x.PrimaryKey))
            {
                tasks.Add(Integration.UpdateBatchAsync(new PartitionKey(entitiesByPartitionKey.Key), entitiesByPartitionKey.Select(x => GetCosmosItem(x)), default));
            }
            await Task.WhenAll(tasks).NoContext();
            return true;
        }
        private ExpandoObject GetCosmosItem(TEntity entity)
        {
            var flexible = new ExpandoObject();
            flexible.TryAdd("id", entity.SecondaryKey);
            flexible.TryAdd(PrimaryKeyName, entity.PrimaryKey);
            flexible.TryAdd("SecondaryKey", entity.SecondaryKey);
            flexible.TryAdd("Timestamp", entity.Timestamp);
            foreach (var property in Properties)
            {
                flexible.TryAdd(property.Name, property.GetValue(entity));
            }
            return flexible;
        }

        public async Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entities)
        {
            List<Task> tasks = new();
            foreach (var entitiesByPartitionKey in entities.GroupBy(x => x.PrimaryKey))
            {
                tasks.Add(Integration.DeleteBatchAsync<TEntity>(entitiesByPartitionKey.Select(x => x.SecondaryKey), new PartitionKey(entitiesByPartitionKey.Key), default));
            }
            await Task.WhenAll(tasks).NoContext();
            return true;
        }

        public string GetName()
            => this.Integration.Configuration.Name;
    }
}