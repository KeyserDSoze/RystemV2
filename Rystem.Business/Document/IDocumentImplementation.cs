using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business.Document
{
    public interface IDocumentImplementation<TEntity>
    {
        Task<bool> ExistsAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAsync(TEntity entity, Expression<Func<TEntity, bool>> expression = default, int? takeCount = default);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(TEntity entity);
        Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entities);
        Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entities);
        string GetName();
    }
}
