using System;
using Microsoft.Extensions.Caching.Memory;

namespace CloudSeedApp {
    public class TSimpleMemoryCache<T> {

        private MemoryCache _cache;
        private SimpleMemoryCacheOptions _options;
        private const int DEFAULT_ENTRY_SIZE = 1;
        
        public TSimpleMemoryCache(
            SimpleMemoryCacheOptions options
        ) {
            

            this._cache = new MemoryCache(
                new MemoryCacheOptions {
                    SizeLimit = options.Size
                }
            );
            this._options = options;
        }

        public bool TryGetValue(string cacheKey, out T value) {
            return this._cache.TryGetValue(cacheKey, out value);
        }

        public void Set(string cacheKey, T value) {
            this._cache.Set(
                cacheKey,
                value,
                new MemoryCacheEntryOptions {
                    Size = DEFAULT_ENTRY_SIZE,
                    SlidingExpiration = this._options.SlidingExpiration,
                    AbsoluteExpiration = DateTimeOffset.UtcNow.Add(this._options.AbsoluteExpiration)
                }
            );
        }

        public void Remove(string cacheKey) {
            this._cache.Remove(cacheKey);
        }
    }

    public class SimpleMemoryCacheOptions {
        public int Size { get; }
        public TimeSpan SlidingExpiration { get; }
        public TimeSpan AbsoluteExpiration { get; }

        public SimpleMemoryCacheOptions(
            int size,
            TimeSpan slidingExpiration,
            TimeSpan absoluteExpiration
        ) {
            if(slidingExpiration > absoluteExpiration) {
                throw new InvalidOperationException("AbsoluteExpiration must be larger than SlidingExpiration!");
            }

            this.Size = size;
            this.SlidingExpiration = slidingExpiration;
            this.AbsoluteExpiration = absoluteExpiration;
        }
    }
}