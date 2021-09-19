using Rystem.Reflection;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business
{
    public interface ICacheKey<TInstance>
    {
        internal const char Separator = '╬';
        internal static readonly Type CacheIgnore = typeof(CacheIgnoreAttribute);
        internal string ToKeyString()
        {
            StringBuilder valueBuilder = new();
            var type = GetType();
            foreach (var property in type.FetchProperties(CacheIgnore))
                valueBuilder.Append($"{property.GetValue(this)}{Separator}");
            return valueBuilder.ToString().Trim(Separator);
        }
        Task<TInstance> FetchAsync();
    }
    public class CacheIgnoreAttribute : Attribute { }
}