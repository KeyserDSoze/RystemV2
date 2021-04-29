using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business.Document
{
    internal class DocumentManager<TEntity>
        where TEntity : IDocument
    {
        //private readonly IDictionary<Installation, INoSqlIntegration<TEntity>> Integrations = new Dictionary<Installation, INoSqlIntegration<TEntity>>();
        //private readonly IDictionary<Installation, NoSqlConfiguration> NoSqlConfiguration;
        //private static readonly object TrafficLight = new object();
        //private DocumentManager<TEntity> Integration(Installation installation)
        //{
        //    if (!Integrations.ContainsKey(installation))
        //        lock (TrafficLight)
        //            if (!Integrations.ContainsKey(installation))
        //            {
        //                NoSqlConfiguration configuration = NoSqlConfiguration[installation];
        //                switch (configuration.Type)
        //                {
        //                    case NoSqlType.TableStorage:
        //                        Integrations.Add(installation, new TableStorageIntegration<TEntity>(configuration, this.DefaultEntity));
        //                        break;
        //                    case NoSqlType.BlobStorage:
        //                        Integrations.Add(installation, new BlobStorageIntegration<TEntity>(configuration, this.DefaultEntity));
        //                        break;
        //                    default:
        //                        throw new InvalidOperationException($"Wrong type installed {configuration.Type}");
        //                }
        //            }
        //    return Integrations[installation];
        //}
        //public InstallerType InstallerType => InstallerType.NoSql;
        //private readonly TEntity DefaultEntity;
        //public DocumentManager(RystemServices configurationBuilder, TEntity entity)
        //{
        //    NoSqlConfiguration = configurationBuilder.GetConfigurations(this.InstallerType).ToDictionary(x => x.Key, x => x.Value as NoSqlConfiguration);
        //    this.DefaultEntity = entity;
        //}
        //public async Task<bool> DeleteAsync(TEntity entity, Installation installation)
        //    => await Integration(installation).DeleteAsync(entity).NoContext();
        //public async Task<bool> DeleteBatchAsync(IEnumerable<TEntity> entity, Installation installation)
        //     => await Integration(installation).DeleteBatchAsync(entity).NoContext();
        //public async Task<bool> ExistsAsync(TEntity entity, Installation installation)
        //    => await Integration(installation).ExistsAsync(entity).NoContext();
        //public async Task<IList<TEntity>> GetAsync(TEntity entity, Installation installation, Expression<Func<TEntity, bool>> expression = null, int? takeCount = null)
        //    => await Integration(installation).GetAsync(entity, expression, takeCount).NoContext();
        //public async Task<bool> UpdateAsync(TEntity entity, Installation installation)
        //    => await Integration(installation).UpdateAsync(entity).NoContext();
        //public async Task<bool> UpdateBatchAsync(IEnumerable<TEntity> entity, Installation installation)
        //    => await Integration(installation).UpdateBatchAsync(entity).NoContext();
        //public string GetName(Installation installation = Installation.Default)
        //    => NoSqlConfiguration[installation].Name;
    }
}
