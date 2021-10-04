namespace Rystem.Background
{
    public interface IAggregationManager<T> : IWarmUp
    {
        void Add(T entity, Installation installation);
        void Flush(Installation installation);
    }
}