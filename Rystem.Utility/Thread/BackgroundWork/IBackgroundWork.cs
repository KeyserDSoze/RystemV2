using Cronos;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public interface IBackgroundWork
    {
        string Key { get; }
        object Properties { get; init; }
        bool RunImmediately { get; }
        string Cron { get; }
        Task ActionToDoAsync();
        private string Id => $"BackgroundWork_{Key ?? string.Empty}_{GetType().FullName}";
        public void Run()
        {
            if (!BackgroundWork.IsRunning(Id))
            {
                var expression = CronExpression.Parse(Cron, Cron.Split(' ').Length > 5 ? CronFormat.IncludeSeconds : CronFormat.Standard);
                BackgroundWork.Run(ActionToDoAsync, Id,
                    () => (int)expression.GetNextOccurrence(DateTime.UtcNow, true)?.Subtract(DateTime.UtcNow).TotalMilliseconds,
                    RunImmediately
                );
            }
        }
        public void Stop()
            => BackgroundWork.Stop(Id);
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