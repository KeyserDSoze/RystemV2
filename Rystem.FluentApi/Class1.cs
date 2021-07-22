using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Rystem.FluentApi
{
    public interface IFluentApi
    {
        string Name { get; }
    }
    public class NoFluentApiAttribute : Attribute { }
    public class FluentApiBuilder
    {
        private readonly IServiceCollection Services;
        internal static FluentApiBuilder Instance { get; set; }
        internal FluentApiBuilder(IServiceCollection services)
            => Services = services;
        public FluentApiBuilder AddApi<T>()
            where T : class, IFluentApi
        {
            Type type = typeof(T);
            Dictionary<string, MethodInfo> methods = type.GetMethods(BindingFlags.Public)
                .Where(x => x.GetCustomAttributes<NoFluentApiAttribute>() == default)
                .ToDictionary(x => $"{type.Name}/{x.Name.Replace("Async", string.Empty)}", x => x);
            
            Services.AddTransient<T>();
            return this;
        }
        private class FluentApiObject
        {
            
            public List<MethodInfo> Methods { get; }
            public FluentApiObject(List<MethodInfo> methods)
                => Methods = methods;
        }
        public IServiceCollection Build()
            => Services;
    }
    public static class FluentApiExtensions
    {
        public static FluentApiBuilder AddFluentApi(this IServiceCollection services)
            => FluentApiBuilder.Instance = new(services);
        public static IApplicationBuilder UseFluentApi(this IApplicationBuilder builder, string prefix = "api")
        {
            builder.UseEndpoints(endpoints =>
            {
                endpoints.Map($"{prefix}/", default //Microsoft.AspNetCore.Http.RequestDelegate
                                            );
            });
            return builder;
        }
    }
    public class FluentApiMiddleware : IMiddleware
    {
        private readonly IServiceProvider ServiceProvider;
        public FluentApiMiddleware(IServiceProvider serviceProvider)
            => ServiceProvider = serviceProvider;
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}