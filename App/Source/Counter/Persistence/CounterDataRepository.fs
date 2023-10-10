namespace CloudSeedApp

open System
open System.Data.Common
open System.Threading.Tasks
open FSharp.Control
open System.Text.Json

open Dapper

open CloudSeedApp.CounterHourly
open CloudSeedApp.CounterEvents

module CounterDataRepository = 

    [<CLIMutable>]
    type CounterPersist = {
        counter_id: string
        timestamp_utc_epoch_ms: int64
        hits: int64
    }

    let tableName = "time_based_counters"

    let mapCounterPersistToDomain (persist : CounterPersist) : CounterHourly = 
        {
            CounterId = persist.counter_id
            TimestampEpochMs = persist.timestamp_utc_epoch_ms
            TotalHits = persist.hits
        }

    let readCounterTotalIOAsync 
        (dbConnection : DbConnection) 
        (counterId : string) 
        (startTimestampUtcEpochMsInclusive : int64) 
        (endTimestampUtcEpochMsInclusive : int64 option) 
        : Async<CounterHourly option> 
        =
        async {
            let effectiveEndTimestamp = 
                match endTimestampUtcEpochMsInclusive with 
                | Some t -> t 
                | None -> startTimestampUtcEpochMsInclusive

            let sql = "SELECT
                counter_id,
                SUM(hits) as hits
                FROM time_based_counters
                WHERE counter_id = @counterId
                    AND timestamp_utc_epoch_ms >= @startTimestampUtcEpochMsInclusive 
                    AND timestamp_utc_epoch_ms <= @endTimestampUtcEpochMsInclusive
                GROUP BY counter_id"

            let queryRecord = ({|
                counterId = counterId
                startTimestampUtcEpochMsInclusive = startTimestampUtcEpochMsInclusive
                endTimestampUtcEpochMsInclusive = effectiveEndTimestamp|})
            // ham - we're not doing a very good job of modeling the domain
            // if we're shoe-horning this stuff in here. REally should
            // use something like counterHitsOverRange or something like that
            let! results = 
                dbConnection.QueryAsync<CounterPersist>(sql, (queryRecord))
                |> Async.AwaitTask

            return results
                |> Seq.map mapCounterPersistToDomain
                |> Seq.tryHead 
        }

    let incrementCounterIOAsync 
        (dbConnection : DbConnection) 
        (counterId : string) 
        (timestampEpochMs : int64) 
        (incrementHits : int64) 
        : Async<unit> 
        =
        async {
            let sql = "INSERT INTO time_based_counters
                (counter_id, timestamp_utc_epoch_ms, hits)
                VALUES
                (@counterId, @timestamp_utc_epoch_ms, @hits)
                ON CONFLICT (counter_id, timestamp_utc_epoch_ms) 
                    DO UPDATE
                    SET hits = time_based_counters.hits + excluded.hits"
            do! dbConnection.ExecuteAsync(sql, ({|
                    counterId = counterId
                    timestamp_utc_epoch_ms = timestampEpochMs
                    hits = incrementHits
                |})) 
                |> Async.AwaitTask
                |> Async.Ignore
        } 
        
