module TestPushTheButtonCommand

open System
open FsUnit
open Xunit

open AppTests.ServiceTree
open CloudSeedApp.GetButtonPushesTotalQuery
open CloudSeedApp.PushTheButtonCommand
open CloudSeedApp.PushTheButtonEvents

// from: https://www.compositional-it.com/news-blog/improved-asynchronous-support-in-f-4-7/
let parallelThrottle throttle workflows =
    Async.Parallel(workflows, throttle)

[<Fact>]
let ``Test PushTheButtonCommand and GetButtonPushesTotalQuery`` () =
    let serviceTree = getServiceTree.Value

    let nowProvider = fun() -> 0L

    let testAsync = async {
        let pushEvent : PushTheButtonCommand = {
            TimestampUtcEpochMs = nowProvider()
            Hits = 1
        }

        let getTotalEvent : GetButtonPushesTotalQueryEvent = {
            StartTimeUtcEpochMs = 0L 
            EndTimeUtcEpochMs = 1L
        }

        let! ogReadResult = getButtonPushesTotalQueryAsync serviceTree.PushTheButtonServiceTree getTotalEvent
        let ogCounter =
            match ogReadResult with
            | Ok r -> r 
            | Error e -> 
                (failwith ("Error retrieving button pushes: " + (string e)))
                0L

        let! _ = sendPushTheButtonCommandAsync ({
            serviceTree.PushTheButtonServiceTree with UtcNowEpochMs = nowProvider}) pushEvent

        // ham: await so queues have time to become eventually consistent
        do! Async.Sleep(2000) 
        
        let! finalReadResult = getButtonPushesTotalQueryAsync serviceTree.PushTheButtonServiceTree getTotalEvent

        match finalReadResult with 
        | Ok r -> 
            r |> should be (greaterThan ogCounter)
        | Error e -> failwith ("Error retrieving button pushes: " + e)
        
    }
    testAsync
        |> Async.RunSynchronously

[<Theory>]
[<InlineData(-1)>]
[<InlineData(101)>]
let ``Test PushTheButtonCommand fails on bad hits`` (badHits : int) =
    let serviceTree = getServiceTree.Value

    let nowProvider = fun() -> 0L

    let testAsync = async {
        let pushEvent : PushTheButtonCommand = {
            TimestampUtcEpochMs = nowProvider()
            Hits = badHits
        }

        let! pushResult = sendPushTheButtonCommandAsync ({
            serviceTree.PushTheButtonServiceTree with UtcNowEpochMs = nowProvider}) pushEvent

        match pushResult with 
        | Ok _ -> failwith "Expected fail on bad hits"
        | Error e -> ()        
    }
    testAsync
        |> Async.RunSynchronously