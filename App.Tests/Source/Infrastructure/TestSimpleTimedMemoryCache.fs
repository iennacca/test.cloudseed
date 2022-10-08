module TestSimpleTimedMemoryCache

open System
open FsUnit
open Xunit

open AppTests.ServiceTree
open CloudSeedApp.SimpleTimedMemoryCache

let simpleCounter (originalValue : int) = 
    let mutable counter = originalValue 

    fun() -> 
        async {
            counter <- counter + 1
            return counter
        }

let simpleStringCounter (originalValue : int) =
    let simpleCounter = simpleCounter originalValue 
    fun() -> 
        async {
            let! counter = simpleCounter()
            return string counter
        }

[<Fact>]
let ``Test SimpleTimedMemoryCache-memo only reads once`` () =
    let simpleIncrementCounter = simpleCounter 0

    let cacheRead = createTimeBasedMemoAsync 2

    for i in 1..10 do 
        cacheRead 0 (simpleIncrementCounter())
        |> Async.RunSynchronously
        |> should equal 1

[<Fact>]
let ``Test SimpleTimedMemoryCache-memo reads after expiry`` () =
    let simpleIncrementCounter = simpleCounter 0

    let timeWindowMs = 2

    let cacheRead = createTimeBasedMemoAsync timeWindowMs

    for i in 0..10 do 
        cacheRead i (simpleIncrementCounter())
        |> Async.RunSynchronously
        // ham: we read once at t = 0, so this must start at 1 read
        |> should equal (1 + (i / timeWindowMs))

[<Fact>]
let ``Test SimpleTimedMemoryCache-memo deals with fetchers of different types`` () =
    let simpleIncrementCounter = simpleCounter 0
    let stringCounter = simpleStringCounter 0

    let timeWindowMs = 0

    let intCacheRead = createTimeBasedMemoAsync timeWindowMs
    let stringCacheRead = createTimeBasedMemoAsync timeWindowMs

    for i in 1..10 do 
        intCacheRead i (simpleIncrementCounter())
        |> Async.RunSynchronously
        |> should equal i

    for i in 1..10 do 
        let stringResult = (stringCacheRead i (stringCounter())
        |> Async.RunSynchronously)
        // printfn "%A" stringResult

        (int stringResult)
        |> should be (greaterThan 0)

[<Fact>]
let ``Test SimpleTimedMemoryCache-memo uses unique caches`` () =
    let return2Async =
        async {
            return 2
        }
    
    let return5Async = 
        async {
            return 5
        }

    let timeWindowMs = 2

    let intCacheReadUnique = createTimeBasedMemoAsync timeWindowMs
    let intCacheReadUnique2 = createTimeBasedMemoAsync timeWindowMs

    intCacheReadUnique 1 (return2Async)
    |> Async.RunSynchronously
    |> should equal 2
    intCacheReadUnique 1 (return5Async)
    |> Async.RunSynchronously
    |> should equal 2

    intCacheReadUnique2 1 (return5Async)
    |> Async.RunSynchronously
    |> should equal 5
    intCacheReadUnique2 1 (return2Async)
    |> Async.RunSynchronously
    |> should equal 5

    let sharedCacheId = (Guid.NewGuid().ToString())
    let intCacheReadShared = createTimeBasedMemoAsync timeWindowMs
    let intCacheReadShared2 = createTimeBasedMemoAsync timeWindowMs

    intCacheReadShared 1 (return2Async)
    |> Async.RunSynchronously
    |> should equal 2
    intCacheReadShared 1 (return5Async)
    |> Async.RunSynchronously
    |> should equal 2

    intCacheReadShared2 1 (return5Async)
    |> Async.RunSynchronously
    |> should equal 5
    intCacheReadShared2 1 (return2Async)
    |> Async.RunSynchronously
    |> should equal 5

[<Fact>]
let ``Test SimpleTimedMemoryCache-cache only reads once`` () =
    let simpleIncrementCounter = simpleCounter 0

    let cacheRead = createTimeBasedCacheAsync 2 10

    for i in 1..10 do 
        cacheRead 0 (string i) (simpleIncrementCounter())
        |> Async.RunSynchronously
        |> should equal i

    for i in 1..10 do 
        cacheRead 0 (string i) (simpleIncrementCounter())
        |> Async.RunSynchronously
        |> should equal i

[<Fact>]
let ``Test SimpleTimedMemoryCache-cache reads after expiry`` () =
    let simpleIncrementCounter = simpleCounter 0

    let timeWindowMs = 2
    let cacheRead = createTimeBasedCacheAsync timeWindowMs 10

    for i in 0..10 do 
        cacheRead i "a" (simpleIncrementCounter())
        |> Async.RunSynchronously
        // ham: we read once at t = 0, so this must start at 1 read
        |> should equal (1 + (i / timeWindowMs))

[<Fact>]
let ``Test SimpleTimedMemoryCache-cache test GC doesn't throw`` () =
    let simpleIncrementCounter = simpleCounter 0

    let timeWindowMs = 2
    let cacheRead = createTimeBasedCacheAsync timeWindowMs 10

    for i in 1..1005 do 
    
    seq {0..1005}
        |> Seq.map (fun i -> 
            cacheRead i (string i) (simpleIncrementCounter()))
        |> Async.Parallel
        |> Async.RunSynchronously
        |> ignore
        // |> Async.Ignore
        // |> should equal (i)


