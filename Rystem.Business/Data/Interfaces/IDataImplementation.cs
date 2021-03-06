using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rystem.Business.Data
{
    internal interface IDataImplementation<TEntity> : IWarmUp
    {
        Task<bool> WriteAsync(string name, Stream stream, dynamic options);
        Task<Stream> ReadAsync(string name);
        Task<IEnumerable<(string Name, Stream Value)>> ListAsync(string filter, int? takeCount = null);
        Task<bool> DeleteAsync(string name);
        Task<bool> ExistsAsync(string name);
        Task<bool> SetPropertiesAsync(string name, dynamic properties);
        Task<List<(string Uri, string Name)>> SearchAsync(string filter, int? takeCount = null);
        bool IsMultipleLines { get; }
        RystemDataServiceProviderOptions Options { get; }
    }
}