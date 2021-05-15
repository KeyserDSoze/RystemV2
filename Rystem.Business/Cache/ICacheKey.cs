using Rystem.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business
{
    public interface ICacheKey<TInstance>
    {
        RystemCacheServiceProvider ConfigureCache();
        internal RystemCacheServiceProvider BuildCache()
            => ConfigureCache().AddInstance(this.GetType());
        private const char Separator = '╬';
        private static readonly Type CacheIgnore = typeof(CacheIgnoreAttribute);
        internal string ToKeyString()
        {
            StringBuilder valueBuilder = new();
            foreach (var property in this.GetType().FetchProperties(CacheIgnore))
                valueBuilder.Append($"{Separator}{property.GetValue(this)}");
            return valueBuilder.ToString();
        }
        Task<TInstance> FetchAsync();
    }
    public class CacheIgnoreAttribute : Attribute { }
}