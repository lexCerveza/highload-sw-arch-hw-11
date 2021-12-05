using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Projectr.RedisClientWrapper
{
    public class RedisClientWrapper
    {
        private readonly Random _random = new Random();
        private readonly IDatabase _redisDb;
        private readonly IStorage _storage;

        public RedisClientWrapper(IConnectionMultiplexer connectionMultiplexer, IStorage storage)
            => (_redisDb, _storage) = (connectionMultiplexer.GetDatabase(), storage);

        public async Task<T> GetAsync<T>(int id, string key, TimeSpan ttl, byte chanceForExpiration = 1)
        {
            string rawItem = await _redisDb.StringGetAsync(key);

            if (rawItem is not null)
            {
                var item = rawItem.FromJson<CacheItem<T>>();

                if (GetEstimatedExpiration(DateTimeOffset.UtcNow, chanceForExpiration, item, _random) < item.ExpirationTime)
                {
                    return item.Value;
                }
            }

            Stopwatch sw = Stopwatch.StartNew();
            T value = await _storage.GetAsync<T>(id);
            sw.Stop();

            var expirationTime = GetExpirationTime(DateTimeOffset.UtcNow, ttl);
            CacheItem<T> cacheItem = new CacheItem<T>(value, sw.ElapsedMilliseconds, expirationTime);

            await _redisDb.StringSetAsync(
                key,
                cacheItem.ToJson(),
                GetEstimatedExpiration<T>(DateTimeOffset.UtcNow, ttl, expirationTime));

            return value;
        }

        private static double GetEstimatedExpiration<T>(DateTimeOffset utcNow, byte chanceForExpiration, CacheItem<T> item, Random random)
            => utcNow.ToUnixTimeMilliseconds() - item.Delta * chanceForExpiration * Math.Log(random.NextDouble());

        private static TimeSpan GetEstimatedExpiration<T>(DateTimeOffset utcNow, TimeSpan ttl, long expirationTime)
            => ttl.Add(TimeSpan.FromMilliseconds(expirationTime - utcNow.ToUnixTimeMilliseconds()));

        private static long GetExpirationTime(DateTimeOffset utcNow, TimeSpan ttl)
            => utcNow.AddMilliseconds(ttl.TotalMilliseconds).ToUnixTimeMilliseconds();

        private record CacheItem<T>(T Value, long Delta, long ExpirationTime);
    }
}