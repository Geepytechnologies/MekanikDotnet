using Microsoft.Extensions.Caching.Memory;

namespace MekanikApi.Infrastructure.Services
{
    public class CacheService(IMemoryCache cache) : ICacheService
    {
        private readonly IMemoryCache _cache = cache;


        public void AddToCache<T>(string key, T value, DateTime expirationTime, Func<Task<(string token, string clientId)>>? callback = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Cache key cannot be null or empty.");
            }

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expirationTime)
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .RegisterPostEvictionCallback(PostEvictionCallback);

            _cache.Set(key, value, cacheOptions);

            if (callback != null)
            {
                FetchNewToken(callback);
            }
        }



        public void AddSmsReminderToCache<T>(string key, T value) { 
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Cache key cannot be null or empty.");
            }

            var cacheOptions = new MemoryCacheEntryOptions()
            {

            };
                

            _cache.Set(key, value, cacheOptions);
        }
        public T? GetFromCache<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Cache key cannot be null or empty.");
            }

            return _cache.TryGetValue(key, out T value) ? value : default;
        }

        public void InvalidateCacheEntry(string key)
        {
            _cache.Remove(key);
        }

        private void PostEvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            Console.WriteLine($"Cache item '{key}' expired. Reason: {reason}");
        }

        public static void FetchNewToken(Func<Task<(string token, string clientId)>> callback)
        {
            Task.Run(async () =>
            {
                var (token, clientId) = await callback();
                Console.WriteLine($"Fetched new token: {token}, clientId: {clientId}");
            });
        }
    }

    public interface ICacheService
    {
        void AddSmsReminderToCache<T>(string key, T value);
        void AddToCache<T>(string key, T value, DateTime expirationTime, Func<Task<(string token, string clientId)>>? callback = null);

        T? GetFromCache<T>(string key);

        void InvalidateCacheEntry(string key);
    }
}