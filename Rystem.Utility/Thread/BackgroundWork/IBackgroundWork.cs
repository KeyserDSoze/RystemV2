using Cronos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public interface IBackgroundWork
    {
        bool ImmediatelyRunning { get; }
        string Cron { get; }
        Task ActionToDo();
    }
    public static partial class BackgroundWorkExtensions
    {
        public static IServiceCollection AddBackgroundWork<TEntity>(this IServiceCollection services)
            where TEntity : IBackgroundWork, new()
        {
            var entity = new TEntity();
            var expression = CronExpression.Parse(entity.Cron, entity.Cron.Split(' ').Length > 5 ? CronFormat.IncludeSeconds : CronFormat.Standard);
            BackgroundWork.Run(entity.ActionToDo, $"IBackgroundWork_{entity.GetType().FullName}",
                () => (int)expression.GetNextOccurrence(DateTime.UtcNow, true)?.Subtract(DateTime.UtcNow).TotalMilliseconds,
                entity.ImmediatelyRunning
            );
            return services;
        }
    }
}