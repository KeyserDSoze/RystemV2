﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rystem.Business.Data
{
    internal interface IDataImplementation<TEntity>
    {
        Task<bool> WriteAsync(TEntity entity, Stream stream, dynamic options);
        Task<Stream> ReadAsync(TEntity entity);
        Task<IEnumerable<(string Name, Stream Value)>> ListAsync(string filter, int takeCount);
        Task<bool> DeleteAsync(TEntity entity);
        Task<bool> ExistsAsync(TEntity entity);
        Task<bool> SetPropertiesAsync(TEntity entity, dynamic properties);
    }
}