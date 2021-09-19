using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rystem.Business.Document
{
    public interface IDocumentManager<TEntity>
        where TEntity : new()
    {
        Task<bool> DeleteAsync(TEntity entity, Installation installation = Installation.Default);
        Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entity, Installation installation = Installation.Default);
        Task<bool> ExistsAsync(TEntity entity, Installation installation = Installation.Default);
        Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default, Installation installation = Installation.Default);
        Task<bool> UpdateAsync(TEntity entity, Installation installation = Installation.Default);
        Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entity, Installation installation = Installation.Default);
        string GetName(Installation installation = Installation.Default);
        Task<TEntity> FirstOrDefaultAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default);
        Task<IEnumerable<TEntity>> ListAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default);
        Task<IEnumerable<TEntity>> TakeAsync(TEntity entity, int takeCount, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default);
        Task<int> CountAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, Installation installation = Installation.Default);
    }
}