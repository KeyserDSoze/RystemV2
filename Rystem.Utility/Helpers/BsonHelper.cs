using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Text
{
    public static class BsonExtensions
    {
        public static string ToBson<T>(this T entity)
        {
            MemoryStream ms = new();
            using (BsonDataWriter writer = new(ms))
                new JsonSerializer().Serialize(writer, entity);
            return Convert.ToBase64String(ms.ToArray());
        }
        public static T FromBson<T>(this string entity)
        {
            byte[] data = Convert.FromBase64String(entity);
            MemoryStream ms = new(data);
            using BsonDataReader reader = new(ms);
            return new JsonSerializer().Deserialize<T>(reader);
        }
        public static async Task<T> FromBson<T>(this Stream entity)
            => (await entity.ConvertToStringAsync().NoContext()).FromBson<T>();
        public static T FromBson<T>(this byte[] entity, JsonSerializerSettings options = default)
            => entity.ConvertToString().FromBson<T>();
    }
}