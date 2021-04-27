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
        public static void ToResult(this Task task)
            => task.NoContext().GetAwaiter().GetResult();
        public static T ToResult<T>(this Task<T> task)
            => task.NoContext().GetAwaiter().GetResult();
        public static void ToResult(this ValueTask task)
            => task.NoContext().GetAwaiter().GetResult();
        public static T ToResult<T>(this ValueTask<T> task)
            => task.NoContext().GetAwaiter().GetResult();
    }
}