using Azure.Storage.Blobs.Models;
using Rystem.Azure.Integration.Storage;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business
{
    internal sealed class InBlobStorage<T> : ICacheImplementation<T>
    {
        private readonly BlobStorageIntegration Integration;
        private readonly string Prefix;
        public InBlobStorage(BlobStorageIntegration integration, string prefix)
        {
            Integration = integration;
            Prefix = prefix;
        }
        private string GetKeyWithPrefix(string key)
            => $"{Prefix}/{key}";
        public async Task<T> InstanceAsync(string key)
        {
            string keyWithPrefix = GetKeyWithPrefix(key);
            var instance = await Integration.ReadAsync(keyWithPrefix).NoContext();
            if (!string.IsNullOrWhiteSpace(instance.Properties.CacheControl) && DateTime.UtcNow > new DateTime(long.Parse(instance.Properties.CacheControl)))
                await this.DeleteAsync(keyWithPrefix).NoContext();
            else
                return (await instance.Content.ConvertToStringAsync().NoContext()).FromJson<T>();
            return default;
        }
        public async Task<bool> UpdateAsync(string key, T value, TimeSpan expiringTime)
        {
            string keyWithPrefix = GetKeyWithPrefix(key);
            long expiring = expiringTime.Ticks;
            await Integration.WriteBlockAsync(keyWithPrefix, await value.ToJson().ToStream().NoContext());
            await Integration.SetBlobPropertiesAsync(keyWithPrefix, new BlobHttpHeaders() { CacheControl = expiring > 0 ? (expiring + DateTime.UtcNow.Ticks).ToString() : string.Empty }).NoContext();
            return true;
        }

        public async Task<bool> DeleteAsync(string key)
            => await Integration.DeleteAsync(GetKeyWithPrefix(key)).NoContext();

        public async Task<CacheStatus<T>> ExistsAsync(string key)
        {
            string keyWithPrefix = GetKeyWithPrefix(key);
            if (await Integration.ExistsAsync(keyWithPrefix).NoContext())
            {
                var value = await Integration.ReadPropertiesAsync(keyWithPrefix).NoContext();
                if (!string.IsNullOrWhiteSpace(value.Properties.CacheControl) && DateTime.UtcNow > new DateTime(long.Parse(value.Properties.CacheControl)))
                {
                    await Integration.DeleteAsync(keyWithPrefix).NoContext();
                    return CacheStatus<T>.NotOk();
                }
                return CacheStatus<T>.Ok();
            }
            return CacheStatus<T>.NotOk();
        }
        public async Task<IEnumerable<string>> ListAsync()
            => (await Integration.ListAsync($"{Prefix}/").NoContext()).Select(x => x.Name);
    }
}