using System;
using System.Threading;
using System.Threading.Tasks;
using CloudSeedApp;
using Xunit;
using Xunit.Extensions;
using Microsoft.EntityFrameworkCore;


namespace AppTests
{
    [Collection(nameof(IntegrationFixtureCollection))]
    public class SanityTestIntegration
    {
        private IntegrationFixture _fixture;

        public SanityTestIntegration(IntegrationFixture fixture) 
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestSanity()
        {
            Assert.NotEqual(1, 2);
        }

        [Fact]
        public async Task TestDatabaseConnection() {
            await _fixture.DbContext
                .Sentinels
                .ToListAsync();
        }

        [Fact]
        public async Task TestMediatorIntegration() {
            var expectedId = Guid.NewGuid().ToString();
            var command = new CreateSentinelCommand { Name = expectedId };

            var createdSentinel = await this._fixture
                .SendAsync(command);
            Assert.Equal(expectedId, createdSentinel.Name);

            var count = await this._fixture
                .DbContext
                .Sentinels
                .CountAsync();
            Assert.True(count > 0);

            // Confirm in db
            await this._fixture
                .DbContext
                .Sentinels
                .SingleAsync(s => s.Name == expectedId);
        }
    }
}
