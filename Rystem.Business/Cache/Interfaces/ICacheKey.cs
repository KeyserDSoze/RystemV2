using Rystem.Reflection;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business
{
    public interface ICacheKey<TInstance>
    {
        internal const char Separator = '╬';
        internal static readonly Type CacheIgnoreKey = typeof(CacheIgnoreKeyAttribute);
        internal string ToKeyString()
        {
            StringBuilder valueBuilder = new();
            var type = GetType();
            foreach (var property in type.FetchProperties(CacheIgnoreKey))
                valueBuilder.Append($"{property.GetValue(this)}{Separator}");
            return valueBuilder.ToString().Trim(Separator);
        }
        Task<TInstance> FetchAsync();
    }
}