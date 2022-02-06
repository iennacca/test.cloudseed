using System;
using System.Threading;
using System.Threading.Tasks;
using CloudSeedApp;
using Xunit;
using Xunit.Extensions;
using Microsoft.EntityFrameworkCore;


namespace AppTests
{
    [Collection(nameof(DatabaseCollectionFixture))]
    public class SanityTest : DatabaseTest
    {
        public SanityTest(DatabaseFixture fixture) : base(fixture) {
        }

        [Fact]
        public void TestSanity()
        {
            Assert.NotEqual(1, 2);
        }

        [Fact]
        public async Task TestDatabaseConnection() {
            await this.DbContext
                .Sentinels
                .ToListAsync();
        }
    }
}
