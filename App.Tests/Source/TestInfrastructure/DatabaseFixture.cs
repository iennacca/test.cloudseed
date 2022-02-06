using System;
using CloudSeedApp;
using Microsoft.EntityFrameworkCore;
using Moq;
// using Npgsql;

namespace AppTests {
    public class DatabaseFixture : IDisposable {
        public CloudSeedAppDatabaseContext DbContext { get; private set; }

        public DatabaseFixture() {
            DbContext = CreateTestDatabaseContext();
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing");
        }

        public static CloudSeedAppDatabaseContext CreateTestDatabaseContext() {
            var connectionString = DatabaseConnectionStringProvider
                .GetConnectionString(
                    Environment.GetEnvironmentVariable(nameof(ConfigurationProvider.DATABASE_HOST)),
                    Environment.GetEnvironmentVariable(nameof(ConfigurationProvider.DATABASE_PASSWORD)),
                    Environment.GetEnvironmentVariable(nameof(ConfigurationProvider.DATABASE_USER)),
                    Environment.GetEnvironmentVariable(nameof(ConfigurationProvider.DATABASE_NAME))
                );
            
            var databaseUpgradeResult = Startup.UpgradeDatabase(
                connectionString
            );
            if(databaseUpgradeResult != 0) {
                throw new Exception("Failed upgrading database!");
            }

            var optionsBuilder = new DbContextOptionsBuilder<CloudSeedAppDatabaseContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new CloudSeedAppDatabaseContext(
                optionsBuilder.Options,
                Mock.Of<IDomainEventService>()
                );

        }
    }
}
