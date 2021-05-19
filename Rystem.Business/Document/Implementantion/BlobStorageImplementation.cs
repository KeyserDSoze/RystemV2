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
        where TEntity : IDocument, new()
    {
        private static readonly Type NoDocumentProperty = typeof(NoDocumentAttribute);
        private readonly Type EntityType;
        private readonly BlobStorageIntegration Integration;
        internal BlobStorageImplementation(BlobStorageIntegration integration, Type entityType)
        {
            Integration = integration;
            this.EntityType = entityType;
        }
        private static string GetBase(TEntity entity)
            => $"{entity.PrimaryKey}/{entity.SecondaryKey}";
        public Task<bool> DeleteAsync(TEntity entity)
            => Integration.DeleteAsync(GetBase(entity));
        public async Task<bool> ExistsAsync(TEntity entity)
            => await Integration.ExistsAsync(GetBase(entity)).NoContext();
        public async Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default)
        {
            string query = string.Empty;
            string secondaryKey = string.Empty;
            if (expression != default && expression.Body is BinaryExpression)
            {
                var bExpr = expression.Body as BinaryExpression;
                dynamic expr = bExpr.Left;
                if (expr.Member.Name == DocumentImplementationConst.PrimaryKey)
                {
                    object rightPart = Expression.Lambda(bExpr.Right).Compile().DynamicInvoke();
                    query = $"{rightPart}/";
                }
                else if (expr.Left.Member.Name == DocumentImplementationConst.SecondaryKey)
                {
                    object rightPart = Expression.Lambda(bExpr.Right).Compile().DynamicInvoke();
                    secondaryKey = rightPart.ToString();
                }
                if (!string.IsNullOrWhiteSpace(query) && !string.IsNullOrWhiteSpace(secondaryKey))
                    query = $"{query}{secondaryKey}";
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