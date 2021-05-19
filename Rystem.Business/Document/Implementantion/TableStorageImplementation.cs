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

namespace Rystem.Business.Document.Implementantion
{
    internal class TableStorageImplementation<TEntity> : IDocumentImplementation<TEntity>
        where TEntity : IDocument, new()
    {
        private readonly Dictionary<string, PropertyInfo> BaseProperties = new();
        private readonly List<PropertyInfo> Properties = new();
        private readonly List<PropertyInfo> SpecialProperties = new();

        private static readonly Type NoDocumentAttribute = typeof(NoDocumentAttribute);
        private readonly Type EntityType;
        private readonly TableStorageIntegration Integration;

        internal TableStorageImplementation(TableStorageIntegration integration, Type entityType)
        {
            Integration = integration;
            this.EntityType = entityType;
            foreach (PropertyInfo pi in this.EntityType.GetProperties())
            {
                if (pi.GetCustomAttribute(NoDocumentAttribute) != default || pi.Name == DocumentImplementationConst.PrimaryKey || pi.Name == DocumentImplementationConst.SecondaryKey || pi.Name == DocumentImplementationConst.Timestamp)
                    continue;
                else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(long) ||
                    pi.PropertyType == typeof(double) || pi.PropertyType == typeof(string) ||
                    pi.PropertyType == typeof(Guid) || pi.PropertyType == typeof(bool) ||
                    pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(byte[]))
                    Properties.Add(pi);
                else
                    SpecialProperties.Add(pi);
            }
        }
        private const string Asterisk = "*";
        private static DynamicTableEntity GetBase(TEntity entity)
            => new()
            {
                PartitionKey = entity.PrimaryKey,
                RowKey = entity.SecondaryKey,
                ETag = Asterisk,
            };
        public Task<bool> DeleteAsync(TEntity entity)
            => Integration.DeleteAsync(GetBase(entity));

        public async Task<bool> ExistsAsync(TEntity entity)
        {
            var result = await Integration.GetAsync(GetBase(entity)).NoContext();
            return result != default;
        }

        public async Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default)
        {
            string query = ToQuery(expression?.Body);

            return (await Integration.QueryAsync(query, takeCount, default)).Select(x => ReadEntity(x));

            string ToQuery(Expression expressionAsExpression = default)
            {
                if (expressionAsExpression == default)
                    return string.Empty;
                string result = QueryStrategy.Create(expressionAsExpression);
                if (!string.IsNullOrWhiteSpace(result))
                    return result;
                BinaryExpression binaryExpression = (BinaryExpression)expressionAsExpression;
                return ToQuery(binaryExpression.Left) + binaryExpression.NodeType.MakeLogic() + ToQuery(binaryExpression.Right);
            }
        }

        public Task<bool> UpdateAsync(TEntity entity)
            => Integration.UpdateAsync(WriteEntity(entity));
        public Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entities)
            => Integration.UpdateBatchAsync(entities.Select(x => WriteEntity(x)));
        public Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entities)
            => Integration.DeleteBatchAsync(entities.Select(x => GetBase(x)));
        public string GetName()
            => this.Integration.Configuration.Name;
        private DynamicTableEntity WriteEntity(TEntity entity)
        {
            DynamicTableEntity dynamicTableEntity = GetBase(entity);
            foreach (PropertyInfo pi in this.Properties)
            {
                dynamic value = pi.GetValue(entity);
                if (value != null)
                    dynamicTableEntity.Properties.Add(pi.Name, new EntityProperty(value));
            }
            foreach (PropertyInfo pi in this.SpecialProperties)
                dynamicTableEntity.Properties.Add(pi.Name, new EntityProperty(
                    pi.GetValue(entity).ToJson()));
            return dynamicTableEntity;
        }
        private static readonly MethodInfo JsonConvertDeserializeMethod = typeof(JsonSerializer).GetMethods(BindingFlags.Public | BindingFlags.Static).First(x => x.IsGenericMethod && x.GetParameters().Any(t => t.ParameterType == typeof(string)));

        private TEntity ReadEntity(DynamicTableEntity dynamicTableEntity)
        {
            TEntity entity = new()
            {
                PrimaryKey = dynamicTableEntity.PartitionKey,
                SecondaryKey = dynamicTableEntity.RowKey,
                Timestamp = dynamicTableEntity.Timestamp.DateTime.ToUniversalTime()
            };
            foreach (PropertyInfo pi in this.Properties)
                if (dynamicTableEntity.Properties.ContainsKey(pi.Name))
                    SetValue(dynamicTableEntity.Properties[pi.Name], pi);
            foreach (PropertyInfo pi in this.SpecialProperties)
                if (dynamicTableEntity.Properties.ContainsKey(pi.Name))
                {
                    dynamic value = JsonConvertDeserializeMethod.MakeGenericMethod(pi.PropertyType).Invoke(null, new object[2] { dynamicTableEntity.Properties[pi.Name].StringValue, null });
                    pi.SetValue(entity, value);
                }
            return entity;
            void SetValue(EntityProperty entityProperty, PropertyInfo pi)
            {
                switch (entityProperty.PropertyType)
                {
                    case EdmType.Int32:
                        pi.SetValue(entity, entityProperty.Int32Value.Value);
                        break;
                    case EdmType.Int64:
                        pi.SetValue(entity, entityProperty.Int64Value.Value);
                        break;
                    case EdmType.Guid:
                        pi.SetValue(entity, entityProperty.GuidValue.Value);
                        break;
                    case EdmType.Double:
                        pi.SetValue(entity, entityProperty.DoubleValue.Value);
                        break;
                    case EdmType.DateTime:
                        pi.SetValue(entity, entityProperty.DateTime.Value);
                        break;
                    case EdmType.Boolean:
                        pi.SetValue(entity, entityProperty.BooleanValue.Value);
                        break;
                    case EdmType.Binary:
                        pi.SetValue(entity, entityProperty.BinaryValue);
                        break;
                    case EdmType.String:
                        pi.SetValue(entity, entityProperty.StringValue);
                        break;
                }
            }
        }
    }
}