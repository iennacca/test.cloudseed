namespace CloudSeedApp

open Microsoft.AspNetCore.Http
open Events
open FSharp.Control
open Giraffe
open System
open System.Threading.Tasks

open CounterServiceTree
open CounterEvents
open CounterDataRepository


module GetCounterTotalQuery =

    let getCounterTotalAsync dbConnection (event : GetCounterTotalQueryEvent) = readCounterTotalIOAsync dbConnection event.CounterId event.StartTimestampUtcEpochMs (Some event.EndTimestampUtcEpochMs)

    let sendGetCounterTotalQueryAsync 
        (serviceTree : CounterServiceTree) 
        (event : GetCounterTotalQueryEvent) 
        : Async<Result<int64, string>> 
        = 
        async {
            use! dbConnection = serviceTree.DbConnectionAsync()

            let cacheSaveId = event.CounterId + ":" + (string event.EndTimestampUtcEpochMs) + ":" + (string event.EndTimestampUtcEpochMs)
            let! counterTotal = serviceTree.CounterReadCache (serviceTree.UtcNowEpochMs()) cacheSaveId (
                getCounterTotalAsync dbConnection event
            )

            return match counterTotal with 
            | Some c -> Ok c.TotalHits
            | None -> Ok 0L
        } 
