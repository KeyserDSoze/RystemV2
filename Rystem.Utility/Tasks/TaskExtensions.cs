using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable NoContext(this Task task)
            => task.ConfigureAwait(false);
        public static ConfiguredTaskAwaitable<T> NoContext<T>(this Task<T> task)
            => task.ConfigureAwait(false);
        public static void ToResult(this Task task)
            => task.NoContext().GetAwaiter().GetResult();
        public static T ToResult<T>(this Task<T> task)
            => task.NoContext().GetAwaiter().GetResult();
    }
}