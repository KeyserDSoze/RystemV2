using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable NoContext(this Task task)
            => task.ConfigureAwait(false);
        public static ConfiguredTaskAwaitable<T> NoContext<T>(this Task<T> task)
            => task.ConfigureAwait(false);
        public static ConfiguredValueTaskAwaitable NoContext(this ValueTask task)
          => task.ConfigureAwait(false);
        public static ConfiguredValueTaskAwaitable<T> NoContext<T>(this ValueTask<T> task)
            => task.ConfigureAwait(false);
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> items)
        {
            List<T> entities = new();
            await foreach (var item in items)
                entities.Add(item);
            return entities;
        }
        public static void ToResult(this Task task)
            => task.NoContext().GetAwaiter().GetResult();
        public static T ToResult<T>(this Task<T> task)
            => task.NoContext().GetAwaiter().GetResult();
        public static void ToResult(this ValueTask task)
            => task.NoContext().GetAwaiter().GetResult();
        public static T ToResult<T>(this ValueTask<T> task)
            => task.NoContext().GetAwaiter().GetResult();

        private static readonly TaskFactory MyTaskFactory = new(CancellationToken.None,
            TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            var cultureUi = CultureInfo.CurrentUICulture;
            var culture = CultureInfo.CurrentCulture;
            return MyTaskFactory.StartNew(() =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUi;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
        }
        public static void RunSync(Func<Task> func)
        {
            var cultureUi = CultureInfo.CurrentUICulture;
            var culture = CultureInfo.CurrentCulture;
            MyTaskFactory.StartNew(() =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUi;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
        }
        public static TResult RunSync<TResult>(Func<ValueTask<TResult>> func) 
            => func.Invoke().ToResult();
        public static void RunSync(Func<ValueTask> func)
            => func.Invoke().ToResult();
    }
}