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
        where TEntity : new()
    {
        private readonly Type EntityType;
        private readonly CosmosNoSqlIntegration Integration;
        private readonly List<PropertyInfo> Properties = new();
        private readonly string PrimaryKeyName = "PrimaryKey";
        private static readonly Type NoDocumentAttribute = typeof(NoDocumentAttribute);
        public RystemDocumentServiceProviderOptions Options { get; }
        public CosmosNoSqlImplementation(CosmosNoSqlIntegration integration, RystemDocumentServiceProviderOptions options)
        {
            Integration = integration;
            Options = options;
            EntityType = typeof(TEntity);
            foreach (PropertyInfo pi in EntityType.GetProperties())
            {
                if (pi.GetCustomAttribute(NoDocumentAttribute) != default || pi.Name == options.PrimaryKey.Name || pi.Name == options.SecondaryKey.Name || pi.Name == options.Timestamp.Name)
                    continue;
                else
                    Properties.Add(pi);
            }
            if (Integration.Configuration.ContainerProperties?.PartitionKeyPath != default)
                PrimaryKeyName = Integration.Configuration.ContainerProperties.PartitionKeyPath.Trim('/');
        }

        public Task<bool> DeleteAsync(TEntity entity)
            => Integration.DeleteAsync<TEntity>(Options.SecondaryKey.GetValue(entity).ToString(), new PartitionKey(Options.PrimaryKey.GetValue(entity).ToString()), default);

        public Task<bool> ExistsAsync(TEntity entity)
            => Integration.ExistAsync<TEntity>(Options.SecondaryKey.GetValue(entity).ToString(), new PartitionKey(Options.PrimaryKey.GetValue(entity).ToString()), default);

        public async Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default)
        {
            List<TEntity> entities = new();
            var primaryKey = Options.PrimaryKey.GetValue(entity);
            await foreach (var value in Integration.ReadAsync(expression, takeCount,
                primaryKey != default ? new QueryRequestOptions { PartitionKey = new PartitionKey(primaryKey.ToString()) } : default,
                default, default))
            {
                entities.Add(value);
            }
            return entities;
        }
        public Task<bool> UpdateAsync(TEntity entity)
        {
            return Integration.UpdateAsync(new PartitionKey(Options.PrimaryKey.GetValue(entity).ToString()), GetCosmosItem(entity), null);
        }

        public async Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entities)
        {
            List<Task> tasks = new();
            foreach (var entitiesByPartitionKey in entities.GroupBy(x => Options.PrimaryKey.GetValue(x).ToString()))
            {
                tasks.Add(Integration.UpdateBatchAsync(new PartitionKey(entitiesByPartitionKey.Key), entitiesByPartitionKey.Select(x => GetCosmosItem(x)), default));
            }
            await Task.WhenAll(tasks).NoContext();
            return true;
        }
        private ExpandoObject GetCosmosItem(TEntity entity)
        {
            var flexible = new ExpandoObject();
            flexible.TryAdd("id", Options.SecondaryKey.GetValue(entity).ToString());
            flexible.TryAdd(PrimaryKeyName, Options.PrimaryKey.GetValue(entity).ToString());
            flexible.TryAdd("SecondaryKey", Options.SecondaryKey.GetValue(entity).ToString());
            flexible.TryAdd("Timestamp", Options.Timestamp.GetValue(entity));
            foreach (var property in Properties)
            {
                flexible.TryAdd(property.Name, property.GetValue(entity));
            }
            return flexible;
        }

        public async Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entities)
        {
            List<Task> tasks = new();
            foreach (var entitiesByPartitionKey in entities.GroupBy(x => Options.PrimaryKey.GetValue(x).ToString()))
            {
                tasks.Add(Integration.DeleteBatchAsync<TEntity>(entitiesByPartitionKey.Select(x => Options.SecondaryKey.GetValue(x).ToString()), new PartitionKey(entitiesByPartitionKey.Key), default));
            }
            await Task.WhenAll(tasks).NoContext();
            return true;
        }

        public string GetName()
            => this.Integration.Configuration.Name;
    }
}