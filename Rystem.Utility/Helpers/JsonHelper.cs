using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rystem.Text
{
    public static class JsonExtensions
    {
        public static string ToJson<T>(this T entity, JsonSerializerSettings options = null)
            => JsonConvert.SerializeObject(entity, options);
        public static T FromJson<T>(this string entity, JsonSerializerSettings options = null)
            => JsonConvert.DeserializeObject<T>(entity, options);
        public static async Task<T> FromJson<T>(this Stream entity, JsonSerializerSettings options = null)
            => (await entity.ConvertToStringAsync().NoContext()).FromJson<T>(options);
        public static T FromJson<T>(this byte[] entity, JsonSerializerSettings options = null)
            => entity.ConvertToString().FromJson<T>(options);
    }
}