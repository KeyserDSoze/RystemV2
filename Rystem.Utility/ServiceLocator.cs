using System;

namespace Rystem
{
    public class ServiceLocator
    {
        public static T GetService<T>() => (T)Services.GetService(typeof(T));
        internal static IServiceProvider Services { get; set; }
    }
}