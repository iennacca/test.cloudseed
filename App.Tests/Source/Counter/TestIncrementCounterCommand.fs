module TestIncrementCounterCommand

open System
open FsUnit
open Xunit

open AppTests.ServiceTree
open CloudSeedApp.IncrementCounterCommand
open CloudSeedApp.CounterEvents
open CloudSeedApp.CounterDataRepository
open CloudSeedApp.CounterTimeUtils

module TestIncrementCounterCommand = 

    // from: https://www.compositional-it.com/news-blog/improved-asynchronous-support-in-f-4-7/
    let parallelThrottle throttle workflows =
        Async.Parallel(workflows, throttle)

    [<Fact>]
    let ``Test IncrementCounterCommand`` () =
        let serviceTree = getServiceTree.Value
        let counterId = Guid.NewGuid().ToString()

        let testAsync = async {
            let event : IncrementCounterCommandEvent = {
                CounterId = counterId
                TimestampUtcEpochMs = (getTimestampEpochMsToHourGranularity (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()))
                Hits = 1
            }
            use! dbConnection = serviceTree.CounterServiceTree.DbConnectionAsync()

            let! ogReadResult = readCounterTotalIOAsync dbConnection counterId event.TimestampUtcEpochMs None
            let ogCounter =
                match ogReadResult with
                | Some r -> r.TotalHits 
                | None -> 0L

            let! pushTheButtonResult = sendIncrementCounterCommandAsync serviceTree.CounterServiceTree event
            
            // we wait to allow for eventual consistency
            do! Async.Sleep(2000)

            let! finalReadResult = readCounterTotalIOAsync dbConnection counterId event.TimestampUtcEpochMs None

            match finalReadResult with 
            | Some r -> 
                r.TotalHits |> should equal (ogCounter + 1L)
            | None -> failwith "No read result found"
            
        }
        testAsync
            |> Async.RunSynchronously

    [<Fact>]
    let ``Test IncrementCounterCommand WorksInParallel`` () =
        let serviceTree = getServiceTree.Value
        let counterId = Guid.NewGuid().ToString()

        let testAsync = async {
            let event : IncrementCounterCommandEvent = {
                CounterId = counterId
                TimestampUtcEpochMs = (getTimestampEpochMsToHourGranularity (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()))
                Hits = 1
            }

            let runTestAsync _i = async {
                use! dbConnection = serviceTree.CounterServiceTree.DbConnectionAsync()

                let! ogReadResult = readCounterTotalIOAsync dbConnection counterId event.TimestampUtcEpochMs None
                let ogCounter =
                    match ogReadResult with
                    | Some r -> r.TotalHits 
                    | None -> 0L

                let! pushTheButtonResult = sendIncrementCounterCommandAsync serviceTree.CounterServiceTree event

                // we wait to allow for eventual consistency
                do! Async.Sleep(2000)
                
                let! readResult = readCounterTotalIOAsync dbConnection counterId event.TimestampUtcEpochMs None

                match readResult with 
                | Some r -> 
                    r.TotalHits |> should be (greaterThan 0L)
                | None -> failwith "No read result found"
            }

            do! (seq { 0..101 }
            |> Seq.map (fun i -> runTestAsync i)
            |> parallelThrottle 10
            |> Async.Ignore)

            use! dbConnection = (serviceTree.CounterServiceTree.DbConnectionAsync())
            let! readResult = readCounterTotalIOAsync dbConnection counterId event.TimestampUtcEpochMs None

            match readResult with 
            | Some r -> 
                r.TotalHits |> should be (greaterThan 100L)
            | None -> failwith "No read result found"
        }
        testAsync
            |> Async.RunSynchronously

