using Azure.Storage.Blobs.Models;
using Rystem.Azure.Integration.Storage;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Rystem.Azure.Integration.Storage.BlobStorageIntegration;

namespace Rystem.Business.Document.Implementantion
{
    internal sealed class BlobStorageImplementation<TEntity> : IDocumentImplementation<TEntity>
    {
        private static readonly Type NoDocumentProperty = typeof(NoDocumentAttribute);
        private readonly BlobStorageIntegration Integration;
        public RystemDocumentServiceProviderOptions Options { get; }
        public BlobStorageImplementation(BlobStorageIntegration integration, RystemDocumentServiceProviderOptions options)
        {
            Integration = integration;
            Options = options;
        }
        private string GetBase(TEntity entity)
            => $"{Options.PrimaryKey.GetValue(entity)}/{Options.SecondaryKey.GetValue(entity)}";
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
                if (expr.Member.Name == Options.PrimaryKey.Name)
                {
                    object rightPart = Expression.Lambda(bExpr.Right).Compile().DynamicInvoke();
                    query = $"{rightPart}/";
                }
                else if (expr.Left.Member.Name == Options.SecondaryKey.Name)
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
        private const string ApplicationJson = "application/json";
        public async Task<bool> UpdateAsync(TEntity entity)
            => await Integration.WriteBlockAsync(GetBase(entity), await entity.ToJson().ToStreamAsync().NoContext(),
                new BlobUploadOptions
                {
                    AccessTier = AccessTier.Hot,
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = ApplicationJson
                    }
                }).NoContext();
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

        public Task<bool> WarmUpAsync()
            => Integration.WarmUpAsync();
    }
}