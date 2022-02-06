using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using CloudSeedApp;


namespace AppTests
{
    [Collection(nameof(DatabaseCollectionFixture))]
    public class TSimpleMemoryCacheTests : DatabaseTest
    {
        public TSimpleMemoryCacheTests(DatabaseFixture fixture) : base(fixture) {

        }

        [Fact]
        public async Task TestCacheHitInt() {
            var cache = new TSimpleMemoryCache<int>(
                new SimpleMemoryCacheOptions(
                    100,
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(60)
                )
            );

            var key = "iamakey";
            var value = 1;

            cache.Set(key, value);

            var cacheResult = cache.TryGetValue(key, out var actualValue);
            Assert.Equal(cacheResult, true);
            Assert.Equal(actualValue, value);
        }

        [Fact]
        public async Task TestCacheMissInt() {
            var cache = new TSimpleMemoryCache<int>(
                new SimpleMemoryCacheOptions(
                    100,
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(60)
                )
            );

            var key = "iamakey";
            var value = 1;

            cache.Set(key, value);

            var cacheResult = cache.TryGetValue(key + "1", out var actualValue);
            Assert.Equal(cacheResult, false);
        }

        [Fact]
        public async Task TestCacheRemovalInt() {
            var cache = new TSimpleMemoryCache<int>(
                new SimpleMemoryCacheOptions(
                    100,
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(60)
                )
            );

            var key = "iamakey";
            var value = 1;

            cache.Set(key, value);

            var cacheResult = cache.TryGetValue(key, out var actualValue);
            Assert.Equal(true, cacheResult);

            cache.Remove(key);

            var cacheResult2 = cache.TryGetValue(key, out var _);
            Assert.Equal(false, cacheResult2);
        }
    }
}
