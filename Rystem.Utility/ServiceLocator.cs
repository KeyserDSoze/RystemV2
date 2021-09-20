using System;

namespace Rystem
{
    public class ServiceLocator
    {
        public static T GetService<T>() => (T)Services.GetService(typeof(T));
        public static object GetService(Type type) => Services.GetService(type);
        internal static IServiceProvider Services { get; set; }
    }
}