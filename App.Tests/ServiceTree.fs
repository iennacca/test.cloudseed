namespace AppTests 

open System

open CloudSeedApp.Configuration
open CloudSeedApp.Persistence
open CloudSeedApp.SentinelPersistence
open CloudSeedApp.ServiceTree
open CloudSeedApp.SimpleTimedMemoryCache

module ServiceTree =

    let buildServiceTreeAndServices (configuration : AppConfiguration) : ServiceTree =
        let connectionString = getDatabaseConnectionString configuration.DATABASE_HOST configuration.DATABASE_NAME configuration.DATABASE_USER configuration.DATABASE_PASSWORD
        let dbConnectionAsync = fun() -> getDbConnectionAsync connectionString

        upgradeDatabase connectionString

        {
            Settings = {
                AppConfiguration = configuration
            }
            SentinelServiceTree = {
                DbContext = fun () -> new SentinelDataContext(connectionString)
            }
        }

    let getServiceTree = lazy(
        let configuration = AppTests.Configuration.fetchConfiguration.Value
        buildServiceTreeAndServices configuration)