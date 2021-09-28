using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Rystem.Cache
{
    public sealed class CacheMiddleware : IMiddleware
    {
        private static readonly Type HttpMethodType = typeof(CachedHttpMethod);
        private readonly CacheOptions Options;
        private readonly ICacheService CacheService;
        public CacheMiddleware(CacheOptions options, ICacheService cacheService)
        {
            Options = options;
            CacheService = cacheService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string uri = context.Request.Path.Value.ToLower();
            CachedHttpMethod method = (CachedHttpMethod)(Enum.Parse(HttpMethodType, context.Request.Method.ToUpperCaseFirst()));
            string key = $"{method}-{uri}";
            bool inCache = await CacheService.ExistsAsync(key).NoContext();
            Stream originalBody = context.Response.Body;
            if (inCache)
            {
                var response = await CacheService.InstanceAsync(key).NoContext();
                context.Response.StatusCode = response.StatusCode;
                context.Response.Headers.TryAdd("R-Status", "Cached");
                foreach (var header in response.Headers)
                    if (!context.Response.Headers.ContainsKey(header.Key))
                        context.Response.Headers.TryAdd(header.Key, header.Value);
                await context.Response.BodyWriter.WriteAsync(response.Body).NoContext();
                await context.Response.CompleteAsync().NoContext();
            }
            else
            {
                context.Response.Body = new MemoryStream();
                await next.Invoke(context);
            }
            if (!inCache)
            {
                var matchingResult = Options.IsMatching(method, uri);
                if (matchingResult.IsMatch)
                {
                    context.Response.Body.Position = 0;
                    Dictionary<string, string> headers = new();
                    foreach (var header in context.Response.Headers)
                        headers.Add(header.Key, header.Value);
                    await CacheService.UpdateAsync(key,
                        new HttpResponseCache(context.Response.StatusCode,
                        headers,
                        (context.Response.Body as MemoryStream).ToArray()),
                            matchingResult.ExpireAfter == default ?
                            Options.DefaultExpireAfter : matchingResult.ExpireAfter
                        ).NoContext();
                    await originalBody.WriteAsync((await CacheService.InstanceAsync(key).NoContext()).Body).NoContext();
                }
            }
        }
    }
}