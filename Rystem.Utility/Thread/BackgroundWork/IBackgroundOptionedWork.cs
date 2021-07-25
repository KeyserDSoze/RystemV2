using System.Threading.Tasks;

namespace Rystem.Background
{
    public interface IBackgroundOptionedWork : IBackgroundWork
    {
        BackgroundWorkOptions Options { get; }
    }
}