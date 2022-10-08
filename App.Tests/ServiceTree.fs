namespace AppTests 

open System

open CloudSeedApp.Configuration
open CloudSeedApp.CounterServiceTree
open CloudSeedApp.GetCounterTotalQuery
open CloudSeedApp.IncrementCounterCommand
open CloudSeedApp.IncrementCounterBatchWriter
open CloudSeedApp.Persistence
open CloudSeedApp.ServiceTree
open CloudSeedApp.SimpleTimedMemoryCache

module ServiceTree =

    let buildServiceTreeAndServices (configuration : AppConfiguration) : ServiceTree =
        let connectionString = getDatabaseConnectionString configuration.DATABASE_HOST configuration.DATABASE_NAME configuration.DATABASE_USER configuration.DATABASE_PASSWORD
        let dbConnectionAsync = fun() -> getDbConnectionAsync connectionString

        let incrementCounterBatchWriter = new IncrementCounterBatchWriter(2, dbConnectionAsync)


        upgradeDatabase connectionString

        let counterServiceTree = {
            CounterReadCache = (createTimeBasedCacheAsync 0 1000)
            DbConnectionAsync = dbConnectionAsync
            IncrementCounterBatchWriter = incrementCounterBatchWriter.Add
            UtcNowEpochMs = fun() -> DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        }

        {
            Settings = {
                AppConfiguration = configuration
            }
            CounterServiceTree = counterServiceTree
            PushTheButtonServiceTree = {
                DbConnectionAsync = dbConnectionAsync
                GetCounterTotalQuery = sendGetCounterTotalQueryAsync counterServiceTree // GetCounterTotalQueryEvent -> Async<Result<int64, string>>
                SendIncrementCounterCommand = sendIncrementCounterCommandAsync counterServiceTree // IncrementCounterCommandEvent -> unit
                UtcNowEpochMs = fun() -> DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            }
            SentinelServiceTree = {
                DbConnectionAsync = dbConnectionAsync
            }
        }

    let getServiceTree = lazy(
        let configuration = AppTests.Configuration.fetchConfiguration.Value
        buildServiceTreeAndServices configuration)