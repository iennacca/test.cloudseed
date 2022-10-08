module TestBatchWriter

open System
open FsUnit
open Xunit

open AppTests.ServiceTree
open CloudSeedApp.BatchWriter

let simpleCounter (originalValue : int) = 
    let mutable counter = originalValue 

    fun() -> 
        async {
            counter <- counter + 1
            return counter
        }

[<Fact>]
let ``Test BatchWriter writes`` () =
    let simpleIncrementCounter = simpleCounter 0

    let writerFnAsync intList = 
        async {
            return! intList
            |> Seq.map (fun _ -> simpleIncrementCounter() |> Async.Ignore)
            |> Async.Sequential
            |> Async.Ignore
        }

    let batchWriter = createBatchWriter<int32> 10 (writerFnAsync)

    let testAsync = async {
        for i in 1..10 do 
            batchWriter i 

        // ham: We sleep a long time to give Mailbox time to write
        Async.Sleep(5000) |> Async.RunSynchronously

        let! count = simpleIncrementCounter()
        count |>  should equal 11
    }

    testAsync
        |> Async.RunSynchronously



