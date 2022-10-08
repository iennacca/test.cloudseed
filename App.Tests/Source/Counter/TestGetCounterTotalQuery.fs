module TestGetCounterTotalQuery

open System
open FsUnit
open Xunit

open AppTests.ServiceTree
open CloudSeedApp.GetCounterTotalQuery
open CloudSeedApp.IncrementCounterCommand
open CloudSeedApp.CounterEvents
open CloudSeedApp.CounterTimeUtils
open CloudSeedApp.IncrementCounterBatchWriter

// from: https://www.compositional-it.com/news-blog/improved-asynchronous-support-in-f-4-7/
let parallelThrottle throttle workflows =
    Async.Parallel(workflows, throttle)

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(2)>]
[<InlineData(11)>]
let ``Test GetCounterTotal returns total`` (expectedTotal : int) =
    let serviceTree = getServiceTree.Value
    let counterId = Guid.NewGuid().ToString()

    let testAsync = async {
        let incrementEvent : IncrementCounterCommandEvent = {
            CounterId = counterId
            TimestampUtcEpochMs = 1L
            Hits = 1
        }

        let getTotalEvent : GetCounterTotalQueryEvent = {
            CounterId = counterId
            StartTimestampUtcEpochMs = 0L
            EndTimestampUtcEpochMs = 2L
        }

        let! ogReadResult = sendGetCounterTotalQueryAsync serviceTree.CounterServiceTree getTotalEvent
        let ogCounter =
            match ogReadResult with
            | Ok r -> r
            | Error e -> failwith ("Failed to read counter total: " + e)
        ogCounter 
            |> should equal 0L

        do! (seq { 0.. (expectedTotal - 1) }
        |> Seq.map (fun i -> (sendIncrementCounterCommandAsync serviceTree.CounterServiceTree incrementEvent))
        |> parallelThrottle 10
        |> Async.Ignore)

        Async.Sleep(5000) 
            |> Async.RunSynchronously

        let! actualReadResult = sendGetCounterTotalQueryAsync serviceTree.CounterServiceTree getTotalEvent
        let actualCounter =
            match actualReadResult with
            | Ok r -> r
            | Error e -> failwith ("Failed to read counter total: " + e)
        
        actualCounter 
            |> should equal (int64 expectedTotal)
    }
    testAsync
        |> Async.RunSynchronously

[<Theory>]
[<InlineData(0, 0, 0)>]
[<InlineData(0, 1, 1)>]
[<InlineData(0, 2, 2)>]
[<InlineData(0, 5, 5)>]
[<InlineData(0, 20, 5)>]
[<InlineData(1, 1, 1)>]
[<InlineData(1, 2, 2)>]
let ``Test GetCounterTotal respects start and end times`` (startTimeDays: int64, endTimeDays: int64, expectedTotal : int) =
    let serviceTree = getServiceTree.Value
    let counterId = Guid.NewGuid().ToString()

    let now = DateTimeOffset.UtcNow

    let testAsync = async {
        // ham - we use day granularity to get closer to proving "hourly" granularity of our cache strategy
        let getTotalEvent : GetCounterTotalQueryEvent = {
            CounterId = counterId
            StartTimestampUtcEpochMs = getTimestampEpochMsToHourGranularity (now.AddDays(float startTimeDays).ToUnixTimeMilliseconds())
            EndTimestampUtcEpochMs = now.AddDays(float endTimeDays).ToUnixTimeMilliseconds()
        }
        let incrementCounterBatchWriter = new IncrementCounterBatchWriter(1, serviceTree.CounterServiceTree.DbConnectionAsync)


        do! (seq { 1 .. 5 }
            |> Seq.map (fun i -> 
                (sendIncrementCounterCommandAsync {
                        serviceTree.CounterServiceTree with IncrementCounterBatchWriter = incrementCounterBatchWriter.Add } {
                    CounterId = counterId
                    TimestampUtcEpochMs = (now.AddDays(float i).ToUnixTimeMilliseconds())
                    Hits = 1
                }))
            |> parallelThrottle 10
            |> Async.Ignore)

        Async.Sleep(2000) 
            |> Async.RunSynchronously

        let! actualReadResult = sendGetCounterTotalQueryAsync serviceTree.CounterServiceTree getTotalEvent
        let actualCounter =
            match actualReadResult with
            | Ok r -> r
            | Error e -> failwith ("Failed to read counter total: " + e)
        
        actualCounter 
            |> should equal (int64 expectedTotal)
    }
    testAsync
        |> Async.RunSynchronously
