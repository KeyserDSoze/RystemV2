using Microsoft.Azure.Cosmos.Table;
using Rystem.Azure.Integration;
using Rystem.Azure.Integration.Storage;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Rystem.Azure.Integration.Storage.BlobStorageIntegration;

namespace Rystem.Business.Document.Implementantion
{
    internal class BlobStorageImplementation<TEntity> : IDocumentImplementation<TEntity>
    {
        private readonly IDictionary<string, PropertyInfo> BaseProperties = new Dictionary<string, PropertyInfo>();
        private readonly List<PropertyInfo> Properties = new();
        private readonly List<PropertyInfo> SpecialProperties = new();
        private const string PartitionKey = "PartitionKey";
        private const string RowKey = "RowKey";
        private const string Timestamp = "Timestamp";
        private static readonly Type NoDocumentProperty = typeof(NoDocumentAttribute);
        private static readonly Type PartitionKeyProperty = typeof(PartitionKeyAttribute);
        private static readonly Type RowKeyProperty = typeof(RowKeyAttribute);
        private static readonly Type TimestampProperty = typeof(TimestampAttribute);
        private readonly Type EntityType;
        private readonly BlobStorageIntegration Integration;
        internal BlobStorageImplementation(BlobStorageIntegration integration, Type entityType)
        {
            Integration = integration;
            this.EntityType = entityType;
            foreach (PropertyInfo pi in this.EntityType.GetProperties())
            {
                if (pi.GetCustomAttribute(NoDocumentProperty) != default)
                    continue;
                if (pi.GetCustomAttribute(PartitionKeyProperty) != default)
                    BaseProperties.Add(PartitionKey, pi);
                else if (pi.GetCustomAttribute(RowKeyProperty) != default)
                    BaseProperties.Add(RowKey, pi);
                else if (pi.GetCustomAttribute(TimestampProperty) != default)
                    BaseProperties.Add(Timestamp, pi);
                else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(long) ||
                    pi.PropertyType == typeof(double) || pi.PropertyType == typeof(string) ||
                    pi.PropertyType == typeof(Guid) || pi.PropertyType == typeof(bool) ||
                    pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(byte[]))
                    Properties.Add(pi);
                else
                    SpecialProperties.Add(pi);
            }
        }
        private string GetBase(TEntity entity)
        {
            object partitionKey = BaseProperties[PartitionKey].GetValue(entity);
            object rowKey = BaseProperties[RowKey].GetValue(entity);
            return $"{partitionKey}/{rowKey}";
        }
        public Task<bool> DeleteAsync(TEntity entity)
            => Integration.DeleteAsync(GetBase(entity));
        public async Task<bool> ExistsAsync(TEntity entity)
            => await Integration.ExistsAsync(GetBase(entity)).NoContext();
        public async Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default)
        {
            string query = string.Empty;
            if (expression != default && expression.Body is BinaryExpression) {
                var bExpr = expression.Body as BinaryExpression;
                if (((dynamic)bExpr.Left).Member.Name == BaseProperties[PartitionKey].Name)
                    query = $"{((dynamic)bExpr.Right).Value}/";
            }
            List<TEntity> entities = new();
            foreach (var item in await Integration.ListAsync(query, takeCount, default))
                entities.Add(await ReadEntity(item).NoContext());
            return entities;
        }
        public async Task<bool> UpdateAsync(TEntity entity)
            => await Integration.WriteBlockAsync(GetBase(entity), await entity.ToJson().ToStream().NoContext()).NoContext();
        public async Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entities)
        {
            List<Task> events = new();
            foreach (var entity in entities)
                events.Add(UpdateAsync(entity));
            await Task.WhenAll(events).NoContext();
            return true;
        }
        public async Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entities)
        {
            List<Task> events = new();
            foreach (var entity in entities)
                events.Add(DeleteAsync(entity));
            await Task.WhenAll(events).NoContext();
            return true;
        }
        public string GetName()
            => Integration.Configuration.Name;
        private static Task<TEntity> ReadEntity(BlobWrapper wrapper)
            => wrapper.Content.FromJsonAsync<TEntity>();
    }
}