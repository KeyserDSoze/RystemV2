using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Bulkhead;
using Polly.Caching;
using Polly.NoOp;
using Polly.Registry;
using System;
using System.Threading.Tasks;

namespace Rystem
{
    public static class PollyExtensions
    {
        private static readonly PolicyRegistry Registry = new();
        public static PolicyBuilder AddRetryPolicy<TException>(this IServiceCollection services, Func<TException, bool> func = default)
            where TException : Exception
        {
            services.TryAddSingleton(Registry);
            return Policy.Handle(func);
        }
        public static PolicyBuilder<TResult> AddRetryWithResultPolicy<TResult>(this IServiceCollection services, Func<TResult, bool> resultPredicate = default)
        {
            services.TryAddSingleton(Registry);
            return Policy.HandleResult(resultPredicate);
        }
        public static NoOpPolicy AddNoOpPolicy(this IServiceCollection services)
        {
            services.TryAddSingleton(Registry);
            return Policy.NoOp();
        }
        public static AsyncBulkheadPolicy AddBulkheadAsyncPolicy(this IServiceCollection services, int maxParallelization, int maxQueuingActions, Func<Context, Task> onBulkheadRejectedAsync)
        {
            services.TryAddSingleton(Registry);
            return Policy.BulkheadAsync(maxParallelization, maxQueuingActions, onBulkheadRejectedAsync);
        }
        public static AsyncBulkheadPolicy<TResult> AddBulkheadAsyncPolicy<TResult>(this IServiceCollection services, int maxParallelization, int maxQueuingActions, Func<Context, Task> onBulkheadRejectedAsync)
        {
            services.TryAddSingleton(Registry);
            return Policy.BulkheadAsync<TResult>(maxParallelization, maxQueuingActions, onBulkheadRejectedAsync);
        }
        public static CachePolicy AddCachePolicy(this IServiceCollection services, ISyncCacheProvider cacheProvider, TimeSpan ttl, Action<Context, string, Exception> onCacheError = null)
        {
            services.TryAddSingleton(Registry);
            return Policy.Cache(cacheProvider, ttl, onCacheError);
        }
        public static IServiceCollection Configure<T>(this T policy, string key = "")
            where T : IsPolicy
        {
            Registry.Add(key, policy);
            return ServiceLocator.Services;
        }
    }
}