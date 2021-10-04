using Rystem.Concurrency;

namespace Rystem
{
    public static class ConfigurableManagerHelper<TEntity, TManager, TProvider>
    {
        private static readonly string RaceConditionKey = $"{typeof(TEntity).Name}-{typeof(TManager).Name}";
        public static TManager ManagerToConfigure(TEntity entity)
        {
            if (entity is IConfigurable<TProvider> configurableEntity)
            {
                RaceCondition.Run(() => configurableEntity.Build(), RaceConditionKey);
                return ServiceLocator.GetService<TManager>();
            }
            else
                return default;
        }
    }
}
