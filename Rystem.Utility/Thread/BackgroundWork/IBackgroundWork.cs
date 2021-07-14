using Cronos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public interface IBackgroundWork
    {
        object Properties { get; init; }
        bool RunImmediately { get; }
        string Cron { get; }
        Task ActionToDoAsync();
        public void Run()
        {
            string id = $"BackgroundWork_{GetType().FullName}";
            if (!BackgroundWork.IsRunning(id))
            {
                var expression = CronExpression.Parse(Cron, Cron.Split(' ').Length > 5 ? CronFormat.IncludeSeconds : CronFormat.Standard);
                BackgroundWork.Run(ActionToDoAsync, id,
                    () => (int)expression.GetNextOccurrence(DateTime.UtcNow, true)?.Subtract(DateTime.UtcNow).TotalMilliseconds,
                    RunImmediately
                );
            }
        }
    }
    public static partial class BackgroundWorkExtensions
    {
        public static IServiceCollection AddBackgroundWork<TEntity>(this IServiceCollection services, Func<object> propertiesRetriever = default)
            where TEntity : IBackgroundWork, new()
        {
            var entity = new TEntity()
            {
                Properties = propertiesRetriever?.Invoke()
            };
            entity.Run();
            return services;
        }
    }
}