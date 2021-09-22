using Rystem.Business;

namespace Rystem.Background
{
    public interface ISequenceManager<T> : IWarmUp
    {
        void Add(T entity, Installation installation);
        void Flush(Installation installation);
    }
}