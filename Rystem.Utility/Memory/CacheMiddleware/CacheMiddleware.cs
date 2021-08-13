using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;

namespace Rystem.Memory
{
    public class CacheMiddleware : IMiddleware
    {
        private static readonly Type HttpMethodType = typeof(CachedHttpMethod);
        private readonly ConcurrentDictionary<string, HttpResponseCache> Responses = new();
        private readonly CacheOptions Options;
        public CacheMiddleware(CacheOptions options)
            => Options = options;
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string uri = context.Request.Path.Value.ToLower();
            CachedHttpMethod method = (CachedHttpMethod)(Enum.Parse(HttpMethodType, context.Request.Method.ToUpperCaseFirst()));
            string key = $"{method}-{uri}";
            bool inCache = false;
            Stream originalBody = context.Response.Body;
            if (inCache = Responses.ContainsKey(key))
            {
                var response = Responses[key];
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
            if (!inCache && Options.IsMatching(method, uri))
            {
                context.Response.Body.Position = 0;
                Dictionary<string, string> headers = new();
                foreach (var header in context.Response.Headers)
                    headers.Add(header.Key, header.Value);
                Responses.TryAdd(key,
                    new HttpResponseCache(context.Response.StatusCode,
                    headers,
                    (context.Response.Body as MemoryStream).ToArray()));
                await originalBody.WriteAsync(Responses[key].Body).NoContext();
            }
        }
    }
}