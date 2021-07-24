using Cronos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public interface IBackgroundOptionedWork : IBackgroundWork
    {
        BackgroundWorkOptions Options { get; }
    }
    public interface IBackgroundWork
    {
        Task ActionToDoAsync();
    }
    public class BackgroundWorkOptions
    {
        public string Key { get; set; }
        public bool RunImmediately { get; set; }
        public string Cron { get; set; }
    }
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
            Start<TEntity>(bOptions);
            return services;
        }
        private static string GetKey<TEntity>(string key)
            where TEntity : class, IBackgroundWork
            => $"BackgroundWork_{key}_{typeof(TEntity).FullName}";
        private static void Start<TEntity>(BackgroundWorkOptions options)
            where TEntity : class, IBackgroundWork
        {
            string key = GetKey<TEntity>(options.Key);
            if (!BackgroundWork.IsRunning(key))
            {
                var expression = CronExpression.Parse(options.Cron, options.Cron.Split(' ').Length > 5 ? CronFormat.IncludeSeconds : CronFormat.Standard);
                BackgroundWork.Run(async () =>
                    {
                        TEntity entity = default;
                        while (entity == null)
                        {
                            try
                            {
                                entity = RystemManager.GetService<TEntity>();
                                break;
                            }
                            catch { }
                            await Task.Delay(1000).NoContext();
                        }
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
            => Start<T>(entity.Options);
        public static void Stop<T>(this T entity)
            where T : class, IBackgroundOptionedWork
            => BackgroundWork.Stop(GetKey<T>(entity.Options.Key));
    }
}