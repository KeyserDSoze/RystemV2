using Rystem;
using Rystem.Business;
using Rystem.Reflection;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace System
{
    public static partial class CacheExtensions
    {
        private static CacheManager<TCacheKey, TCache> Manager<TCacheKey, TCache>(this TCacheKey entity)
            where TCacheKey : ICacheKey<TCache>
            => ServiceLocator.GetService<CacheManager<TCacheKey, TCache>>();

        public static async Task<TEntry> InstanceAsync<TEntry>(this ICacheKey<TEntry> entry, TimeSpan expiringTime = default, Installation installation = Installation.Default, bool withConsistency = false)
            => await entry.Manager<ICacheKey<TEntry>, TEntry>().InstanceAsync(entry, withConsistency, expiringTime, installation).NoContext();
        public static async Task<bool> RemoveAsync<TEntry>(this ICacheKey<TEntry> entry, Installation installation = Installation.Default)
            => await entry.Manager<ICacheKey<TEntry>, TEntry>().DeleteAsync(entry, installation).NoContext();
        public static async Task<bool> RestoreAsync<TEntry>(this ICacheKey<TEntry> entry, TEntry value = default, TimeSpan expiringTime = default, Installation installation = Installation.Default)
           => await entry.Manager<ICacheKey<TEntry>, TEntry>().UpdateAsync(entry, value, expiringTime, installation).NoContext();
        public static async Task<bool> IsPresentAsync<TEntry>(this ICacheKey<TEntry> entry, Installation installation = Installation.Default)
           => await entry.Manager<ICacheKey<TEntry>, TEntry>().ExistsAsync(entry, installation).NoContext();
        public static async Task<IList<ICacheKey<TEntry>>> KeysAsync<TEntry>(this ICacheKey<TEntry> entry, Installation installation = Installation.Default)
        {
            Type keyType = entry.GetType();
            IList<ICacheKey<TEntry>> keys = new List<ICacheKey<TEntry>>();
            foreach (string key in await entry.Manager<ICacheKey<TEntry>, TEntry>().ListAsync(installation))
            {
                ICacheKey<TEntry> multitonKey = (ICacheKey<TEntry>)Activator.CreateInstance(keyType);
                IEnumerator<string> keyValues = PropertyValue(key);
                if (!keyValues.MoveNext())
                    continue;
                foreach (PropertyInfo property in keyType.FetchProperties(ICacheKey<TEntry>.CacheIgnore))
                {
                    property.SetValue(multitonKey, Convert.ChangeType(keyValues.Current, property.PropertyType));
                    if (!keyValues.MoveNext())
                        break;
                }
                keys.Add(multitonKey);
            }
            return keys;

            static IEnumerator<string> PropertyValue(string key)
            {
                foreach (string s in key.Split(ICacheKey<TEntry>.Separator))
                    yield return s;
            }
        }

        public static TEntry Instance<TEntry>(this ICacheKey<TEntry> entry, TimeSpan expiringTime = default, Installation installation = Installation.Default, bool withConsistency = false)
            => entry.InstanceAsync(expiringTime, installation, withConsistency).ToResult();
        public static bool Remove<TEntry>(this ICacheKey<TEntry> entry, Installation installation = Installation.Default)
            => entry.RemoveAsync(installation).ToResult();
        public static bool Restore<TEntry>(this ICacheKey<TEntry> entry, TEntry value = default, TimeSpan expiringTime = default, Installation installation = Installation.Default)
           => entry.RestoreAsync(value, expiringTime, installation).ToResult();
        public static bool IsPresent<TEntry>(this ICacheKey<TEntry> entry, Installation installation = Installation.Default)
            => entry.IsPresentAsync(installation).ToResult();
        public static IList<ICacheKey<TEntry>> Keys<TEntry>(this ICacheKey<TEntry> entry, Installation installation = Installation.Default)
            => entry.KeysAsync(installation).ToResult();
    }
}