using System;
using Npgsql;

namespace CloudSeedApp {
    public class DatabaseConnectionStringProvider {
        public static string GetConnectionStringFromConfigurationProvider(ConfigurationProvider configuration) {
            return GetConnectionString(
                configuration.DATABASE_HOST,
                configuration.DATABASE_PASSWORD,
                configuration.DATABASE_USER,
                configuration.DATABASE_NAME
            );
        }

        public static string GetConnectionString(
            string databaseHost,
            string databasePassword,
            string databaseUser,
            string databaseName
        ) {
            var connectionString = new NpgsqlConnectionStringBuilder() 
            {
                SslMode = SslMode.Disable,
                Host = databaseHost,
                Username = databaseUser,
                Password = databasePassword,
                Database = databaseName
            };
            connectionString.Pooling = true;
            

            // var connectionString = $"Host={databaseHost};User Id={databaseUser};Password={databasePassword};Database={databaseName};Port=5432";

            return connectionString.ToString();
        }
    }
}