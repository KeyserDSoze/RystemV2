namespace Rystem.Background
{
    public static class AggregationExtensions
    {
        private static IAggregationManager<T> Manager<T>(this T entity)
            where T : IAggregation
            => ServiceLocator.GetService<IAggregationManager<T>>();
        public static void Add<T>(this T key, T entity, Installation installation = Installation.Default)
            where T : IAggregation
            => key.Manager().Add(entity, installation);
        public static void Flush<T>(this T key, Installation installation = Installation.Default)
            where T : IAggregation
            => key.Manager().Flush(installation);
    }
}