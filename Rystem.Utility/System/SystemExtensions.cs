using Newtonsoft.Json;

namespace Rystem
{
    public static class SystemExtensions
    {
        private static readonly JsonSerializerSettings Options = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.Auto,
        };
        public static T DeepCopy<T>(this T original)
            => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(original, Options), Options);
    }
}