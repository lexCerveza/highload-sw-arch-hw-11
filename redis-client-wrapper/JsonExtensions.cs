using Newtonsoft.Json;

namespace Projectr.RedisClientWrapper
{
    public static class JsonExtensions
    {
        public static T FromJson<T>(this string rawJson) => JsonConvert.DeserializeObject<T>(rawJson);
        public static string ToJson<T>(this T obj) => JsonConvert.SerializeObject(obj);
    }
}