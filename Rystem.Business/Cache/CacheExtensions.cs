﻿using Rystem.Business;
using Rystem.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static partial class CacheExtensions
    {
        private static CacheManager<TCacheKey, TCache> Manager<TCacheKey, TCache>(this TCacheKey entity)
            where TCacheKey : ICacheKey<TCache>
            where TCache : new()
            => entity.DefaultManager(nameof(CacheExtensions), (key) => new CacheManager<TCacheKey, TCache>(key.BuildCache())) as CacheManager<TCacheKey, TCache>;

        public static async Task<TEntry> InstanceAsync<TEntry>(this ICacheKey<TEntry> entry, bool withConsistency = false)
            where TEntry : new()
            => await entry.Manager<ICacheKey<TEntry>, TEntry>().InstanceAsync(entry, withConsistency).NoContext();
        public static async Task<bool> RemoveAsync<TEntry>(this ICacheKey<TEntry> entry)
            where TEntry : new()
            => await entry.Manager<ICacheKey<TEntry>, TEntry>().DeleteAsync(entry).NoContext();
        public static async Task<bool> RestoreAsync<TEntry>(this ICacheKey<TEntry> entry, TEntry value = default, TimeSpan expiringTime = default)
            where TEntry : new()
           => await entry.Manager<ICacheKey<TEntry>, TEntry>().UpdateAsync(entry, value, expiringTime).NoContext();
        public static async Task<bool> IsPresentAsync<TEntry>(this ICacheKey<TEntry> entry)
            where TEntry : new()
           => await entry.Manager<ICacheKey<TEntry>, TEntry>().ExistsAsync(entry).NoContext();
        public static async Task<IList<ICacheKey<TEntry>>> KeysAsync<TEntry>(this ICacheKey<TEntry> entry)
            where TEntry : new()
        {
            Type keyType = entry.GetType();
            IList<ICacheKey<TEntry>> keys = new List<ICacheKey<TEntry>>();
            foreach (string key in await entry.Manager<ICacheKey<TEntry>, TEntry>().ListAsync())
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

        public static TEntry Instance<TEntry>(this ICacheKey<TEntry> entry, bool withConsistency = false)
            where TEntry : new()
            => entry.InstanceAsync(withConsistency).ToResult();
        public static bool Remove<TEntry>(this ICacheKey<TEntry> entry)
            where TEntry : new()
            => entry.RemoveAsync().ToResult();
        public static bool Restore<TEntry>(this ICacheKey<TEntry> entry, TEntry value = default, TimeSpan expiringTime = default)
            where TEntry : new()
           => entry.RestoreAsync(value, expiringTime).ToResult();
        public static bool IsPresent<TEntry>(this ICacheKey<TEntry> entry)
            where TEntry : new()
            => entry.IsPresentAsync().ToResult();
        public static IList<ICacheKey<TEntry>> Keys<TEntry>(this ICacheKey<TEntry> entry)
            where TEntry : new()
            => entry.KeysAsync().ToResult();
    }
}
