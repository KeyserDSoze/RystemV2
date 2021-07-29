using Cronos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public static partial class BackgroundWorkExtensions
    {
        public static IServiceCollection AddBackgroundWork<TEntity>(this IServiceCollection services, Action<BackgroundWorkOptions> options)
            where TEntity : class, IBackgroundWork
        {
            services.AddTransient<TEntity>();
            var bOptions = new BackgroundWorkOptions()
            {
                Key = Guid.NewGuid().ToString(),
                Cron = "0 1 * * *",
                RunImmediately = false
            };
            options.Invoke(bOptions);
            Start<TEntity>(default, bOptions);
            return services;
        }
        private static string GetKey<TEntity>(string key)
            where TEntity : class, IBackgroundWork
            => $"BackgroundWork_{key}_{typeof(TEntity).FullName}";
        private static void Start<TEntity>(TEntity entity, BackgroundWorkOptions options)
            where TEntity : class, IBackgroundWork
        {
            string key = GetKey<TEntity>(options.Key);
            if (!BackgroundWork.IsRunning(key))
            {
                var expression = CronExpression.Parse(options.Cron, options.Cron.Split(' ').Length > 5 ? CronFormat.IncludeSeconds : CronFormat.Standard);
                BackgroundWork.Run(async () =>
                    {
                        if (entity == default)
                        {
                            TEntity entityFromServices = default;
                            int attempt = 0;
                            while (entityFromServices == null || attempt > 30)
                            {
                                try
                                {
                                    entityFromServices = RystemManager.GetService<TEntity>();
                                    break;
                                }
                                catch
                                {
                                    attempt++;
                                }
                                await Task.Delay(300).NoContext();
                            }
                            await entityFromServices.ActionToDoAsync().NoContext();
                        }
                        else
                            await entity.ActionToDoAsync().NoContext();
                    },
                    key,
                    () => (int)expression.GetNextOccurrence(DateTime.UtcNow, true)?.Subtract(DateTime.UtcNow).TotalMilliseconds,
                    options.RunImmediately
                );
            }
        }
        public static void Run<T>(this T entity)
            where T : class, IBackgroundOptionedWork
            => Start(entity, entity.Options);
        public static void Stop<T>(this T entity)
            where T : class, IBackgroundOptionedWork
            => BackgroundWork.Stop(GetKey<T>(entity.Options.Key));
        public static void Stop<T>(this T entity, string key)
           where T : class, IBackgroundWork
            => BackgroundWork.Stop(GetKey<T>(key));
    }
}