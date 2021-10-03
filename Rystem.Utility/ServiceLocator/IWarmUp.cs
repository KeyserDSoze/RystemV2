using System.Threading.Tasks;

namespace Rystem
{
    public interface IWarmUp
    {
        Task<bool> WarmUpAsync();
    }
}