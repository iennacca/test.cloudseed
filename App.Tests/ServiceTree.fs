namespace AppTests 

open CloudSeedApp.Configuration
open CloudSeedApp.Persistence
open CloudSeedApp.ServiceTree

module ServiceTree =

    let buildServiceTreeAndServices (configuration : AppConfiguration) : ServiceTree =
        let connectionString = getDatabaseConnectionString configuration.DATABASE_HOST configuration.DATABASE_NAME configuration.DATABASE_USER configuration.DATABASE_PASSWORD
        let dbConnection = fun() -> getDatabaseConnection connectionString

        upgradeDatabase connectionString

        {
            Settings = {
                AppConfiguration = configuration
            }
            SentinelServiceTree = {
                // Workflows = {
                //     GetSentinelQuery = sendGetSentinelQueryAsync 
                // }
                WorkflowIOs = {
                    DbConnection = dbConnection
                }
            }
            WorkflowIOs = {
                NotAnIO = 1
                getDbConnection = dbConnection
            }
            Workflows = {
                NotAWorkflow = 1
            }
        }

    let getServiceTree = lazy(
        let configuration = AppTests.Configuration.fetchConfiguration.Value
        buildServiceTreeAndServices configuration)