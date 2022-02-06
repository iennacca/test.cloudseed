using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using Microsoft.EntityFrameworkCore;

using CloudSeedApp;


namespace AppTests
{
    [Collection(nameof(DatabaseCollectionFixture))]
    public class ConfigurationProviderTests : DatabaseTest
    {
        public ConfigurationProviderTests(DatabaseFixture fixture) : base(fixture) {

        }

        [Fact]
        public async Task TestEnvironmentVariableConfiguration() {
            var configuration = TestDataPopulationUtilities.GetConfigurationProvider();

            foreach(PropertyInfo property in typeof(ConfigurationProvider).GetProperties()) {
                Assert.True(property.GetValue(configuration) is not null, $"Configuration property is null! Name: {property.Name}");
            }
        }

    }
}
