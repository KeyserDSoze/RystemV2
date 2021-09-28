using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Rystem.Background;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem
{
    public static partial class BackgroundJobExtensions
    {
        public static IServiceCollection AddBackgroundJob<TEntity>(this IServiceCollection services, Action<BackgroundJobOptions> options, Action<Exception> onError = default)
            where TEntity : class, IBackgroundJob
        {
            services.AddTransient<TEntity>();
            var bOptions = new BackgroundJobOptions()
            {
                Key = Guid.NewGuid().ToString(),
                Cron = "0 1 * * *",
                RunImmediately = false
            };
            options.Invoke(bOptions);
            services.AddRystemFullyAddedCallback(() => ThreadPool.QueueUserWorkItem(x => Start<TEntity>(default, bOptions)));
            return services;
        }
        private static string GetKey<TEntity>(string key)
            where TEntity : class, IBackgroundJob
            => $"BackgroundWork_{key}_{typeof(TEntity).FullName}";
        private static void Start<TEntity>(TEntity entity, BackgroundJobOptions options)
            where TEntity : class, IBackgroundJob
        {
            string key = GetKey<TEntity>(options.Key);
            if (!BackgroundJob.IsRunning(key))
            {
                var expression = CronExpression.Parse(options.Cron, options.Cron.Split(' ').Length > 5 ? CronFormat.IncludeSeconds : CronFormat.Standard);
                BackgroundJob.Run(async () =>
                    {
                        int attempt = 0;
                        while (entity == default && attempt < 30)
                        {
                            try
                            {
                                entity = ServiceLocator.GetService<TEntity>();
                                if (entity != default)
                                    break;
                                else
                                    attempt++;
                            }
                            catch (Exception exception)
                            {
                                attempt++;
                                if (attempt >= 30)
                                    throw new NullReferenceException($"{typeof(TEntity).FullName} can't be retrieved from Service Collection. Please check your startup. Error: {exception.Message}");
                            }
                            await Task.Delay(300).NoContext();
                        }
                        try
                        {
                            await entity.ActionToDoAsync().NoContext();
                        }
                        catch (Exception exception)
                        {
                            await entity.OnException(exception).NoContext();
                        }
                    },
                    key,
                    () => expression.GetNextOccurrence(DateTime.UtcNow, true)?.Subtract(DateTime.UtcNow).TotalMilliseconds ?? 120,
                    options.RunImmediately
                );
            }
        }
        public static void Run<T>(this T entity)
            where T : class, IBackgroundOptionedJob
            => Start(entity, entity.Options);
        public static void Stop<T>(this T entity)
            where T : class, IBackgroundOptionedJob
            => BackgroundJob.Stop(GetKey<T>(entity.Options.Key));
        public static void Stop<T>(this T entity, string key)
           where T : class, IBackgroundJob
            => BackgroundJob.Stop(GetKey<T>(key));
    }
}