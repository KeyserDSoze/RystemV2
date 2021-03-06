using Rystem;
using Rystem.Business;
using Rystem.Business.Document;
using Rystem.Concurrency;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace System
{
    public static class DocumentExtensions
    {
        private static IDocumentManager<TEntity> Manager<TEntity>(this TEntity entity)
            where TEntity : IDocument
            => ServiceLocator.GetService<IDocumentManager<TEntity>>() ??
                    ConfigurableManagerHelper<TEntity, IDocumentManager<TEntity>, RystemDocumentServiceProvider>.ManagerToConfigure(entity);
        
        public static async Task<bool> UpdateAsync<TEntity>(this TEntity entity, Installation installation = Installation.Default)
            where TEntity : IDocument
           => await entity.Manager().UpdateAsync(entity, installation).NoContext();
        public static async Task<bool> UpdateBatchAsync<TEntity>(this IEnumerable<TEntity> entities, Installation installation = Installation.Default)
            where TEntity : IDocument
        {
            bool result = true;
            foreach (var ents in entities.GroupBy(x => x.GetType().FullName))
                result &= await ents.FirstOrDefault().Manager().UpdateBatchAsync(ents, installation).NoContext();
            return result;
        }
        public static async Task<bool> DeleteAsync<TEntity>(this TEntity entity, Installation installation = Installation.Default)
            where TEntity : IDocument
           => await entity.Manager().DeleteAsync(entity, installation).NoContext();
        public static async Task<bool> DeleteBatchAsync<TEntity>(this IEnumerable<TEntity> entities, Installation installation = Installation.Default)
            where TEntity : IDocument
        {
            bool result = true;
            foreach (var ents in entities.GroupBy(x => x.GetType().FullName))
                result &= await ents.FirstOrDefault().Manager().DeleteBatchAsync(ents, installation).NoContext();
            return result;
        }
        public static async Task<bool> ExistsAsync<TEntity>(this TEntity entity, Installation installation = Installation.Default)
            where TEntity : IDocument
           => await entity.Manager().ExistsAsync(entity, installation).NoContext();
        public static async Task<IEnumerable<TEntity>> GetAsync<TEntity>(this TEntity entity, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default, Installation installation = Installation.Default)
            where TEntity : IDocument
           => await entity.Manager().GetAsync(entity, expression, takeCount, installation).NoContext();
        public static bool Update<TEntity>(this TEntity entity, Installation installation = Installation.Default)
           where TEntity : IDocument
          => UpdateAsync(entity, installation).ToResult();
        public static bool UpdateBatch<TEntity>(this IEnumerable<TEntity> entities, Installation installation = Installation.Default)
           where TEntity : IDocument
          => UpdateBatchAsync(entities, installation).ToResult();
        public static bool Delete<TEntity>(this TEntity entity, Installation installation = Installation.Default)
            where TEntity : IDocument
           => DeleteAsync(entity, installation).ToResult();
        public static bool DeleteBatch<TEntity>(this IEnumerable<TEntity> entities, Installation installation = Installation.Default)
           where TEntity : IDocument
          => DeleteBatchAsync(entities, installation).ToResult();
        public static bool Exists<TEntity>(this TEntity entity, Installation installation = Installation.Default)
            where TEntity : IDocument
           => ExistsAsync(entity, installation).ToResult();
        public static IEnumerable<TEntity> Get<TEntity>(this TEntity entity, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default, Installation installation = Installation.Default)
            where TEntity : IDocument
           => GetAsync(entity, expression, takeCount, installation).ToResult();
        public static string GetName<TEntity>(this TEntity entity, Installation installation = Installation.Default)
            where TEntity : IDocument
        => entity.Manager().GetName(installation);
        public static Task<TEntity> FirstOrDefaultAsync<TEntity>(this TEntity entity, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default)
            where TEntity : IDocument
           => entity.Manager().FirstOrDefaultAsync(entity, expression, installation);
        public static Task<IEnumerable<TEntity>> ListAsync<TEntity>(this TEntity entity, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default)
            where TEntity : IDocument
           => entity.Manager().ListAsync(entity, expression, installation);
        public static Task<IEnumerable<TEntity>> TakeAsync<TEntity>(this TEntity entity, int takeCount, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default)
            where TEntity : IDocument
           => entity.Manager().TakeAsync(entity, takeCount, expression, installation);
        public static Task<int> CountAsync<TEntity>(this TEntity entity, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default)
           where TEntity : IDocument
          => entity.Manager().CountAsync(entity, expression, installation);
    }
}