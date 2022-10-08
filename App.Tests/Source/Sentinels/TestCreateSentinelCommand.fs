module TestCreateSentinelCommand

open System
open FsUnit
open Xunit

open CloudSeedApp.CreateSentinelCommand
open CloudSeedApp.GetSentinelQuery
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


