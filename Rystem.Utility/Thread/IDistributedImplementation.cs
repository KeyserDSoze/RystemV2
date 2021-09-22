using System.Threading.Tasks;

namespace Rystem.Concurrency
{
    public interface IDistributedImplementation: IWarmUp
    {
        Task<bool> AcquireAsync(string key);
        Task<bool> IsAcquiredAsync(string key);
        Task<bool> ReleaseAsync(string key);
    }
}