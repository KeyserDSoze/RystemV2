using Cronos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public interface IBackgroundWork
    {
        bool RunImmediately { get; }
        string Cron { get; }
        Task ActionToDo();
        public void Run()
        {
            string id = $"IBackgroundWork_{GetType().FullName}";
            if (!BackgroundWork.IsRunning(id))
            {
                var expression = CronExpression.Parse(Cron, Cron.Split(' ').Length > 5 ? CronFormat.IncludeSeconds : CronFormat.Standard);
                BackgroundWork.Run(ActionToDo, $"IBackgroundWork_{GetType().FullName}",
                    () => (int)expression.GetNextOccurrence(DateTime.UtcNow, true)?.Subtract(DateTime.UtcNow).TotalMilliseconds,
                    RunImmediately
                );
            }
        }
    }
    public static partial class BackgroundWorkExtensions
    {
        public static IServiceCollection AddBackgroundWork<TEntity>(this IServiceCollection services)
            where TEntity : IBackgroundWork, new()
        {
            var entity = new TEntity();
            entity.Run();
            return services;
        }
    }
}