module TestCreateSentinelCommand

open System
open FsUnit
open Xunit

open CloudSeedApp.SentinelCommands
open CloudSeedApp.SentinelDomain
open CloudSeedApp.SentinelQueries
open AppTests.ServiceTree

[<Fact>]
let ``Test Create Sentinel Command`` () =
    let serviceTree = getServiceTree.Value

    let testAsync = async {
        let! createCommandResult = sendCreateSentinelCommandAsync serviceTree.SentinelServiceTree
        match createCommandResult with 
            | Ok r -> 
                r.id |> should not' (be null)
                r.id.Length |> should be (greaterThan 5)
                let! queryResult = sendGetSentinelQueryAsync serviceTree.SentinelServiceTree { id = r.id }
                match queryResult with 
                    | Error _ -> failwith "No DB Sentinel found"
                    | Ok q -> true |> should equal true
            | Error _ -> true |> should equal false

        
    }
    testAsync
        |> Async.RunSynchronously


[<Fact>]
let ``Test Sentinel Query - Does Not Exist`` () =
    let serviceTree = getServiceTree.Value

    let doesNotExistId = "THIS_ID_DOES_NOT_EXIST"

    let testAsync = async {
        let! sentinelResult = sendGetSentinelQueryAsync serviceTree.SentinelServiceTree { id = doesNotExistId }
        match sentinelResult with 
            | Ok r -> 
                failwith "This id should not exist!"
            | Error e ->
                e 
                |> should equal GetSentinelQueryErrors.NoSentinelFound
        
    }
    testAsync
        |> Async.RunSynchronously