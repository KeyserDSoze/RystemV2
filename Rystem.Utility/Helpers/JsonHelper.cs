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
        public static string ToJson<T>(this T entity, JsonSerializerOptions options = default)
            => JsonSerializer.Serialize(entity, options);
        public static T FromJson<T>(this string entity, JsonSerializerOptions options = default)
            => JsonSerializer.Deserialize<T>(entity, options);
        public static async Task<T> FromJsonAsync<T>(this Stream entity, JsonSerializerOptions options = default)
            => (await entity.ConvertToStringAsync().NoContext()).FromJson<T>(options);
        public static T FromJson<T>(this byte[] entity, JsonSerializerOptions options = default)
            => entity.ConvertToString().FromJson<T>(options);
    }
}