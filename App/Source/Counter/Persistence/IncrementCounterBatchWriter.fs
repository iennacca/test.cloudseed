namespace CloudSeedApp

open System.Data.Common

open BatchWriter
open CounterDataRepository
open CounterEvents

module IncrementCounterBatchWriter = 
    
    type IncrementCounterBatchWriter(
        batchSize: int, 
        createDbConnectionAsync : unit -> Async<DbConnection>) 
        = 

        let batchSize = batchSize
        let createDbConnectionAsync = createDbConnectionAsync

        let batchIncrementEvents 
            (incrementEventList : IncrementCounterCommandEvent list) 
            : IncrementCounterCommandEvent list 
            = 
            let counterToEventsLookup = 
                incrementEventList
                |> Seq.groupBy (fun e -> e.CounterId)
                |> Map.ofSeq

            let counterToSmallestTimeLookup = 
                counterToEventsLookup
                |> Map.keys
                |> Seq.map (fun k -> 
                    counterToEventsLookup[k]
                    |> (Seq.minBy (fun v -> v.TimestampUtcEpochMs)))
                |> Seq.map (fun e -> (e.CounterId, e.TimestampUtcEpochMs))
                |> Map.ofSeq

            let counterToIncrementSums = 
                counterToEventsLookup
                |> Map.keys 
                |> Seq.map (fun k -> 
                    let sum = 
                        counterToEventsLookup[k]
                        |> Seq.sumBy (fun e -> e.Hits)
                    (k, sum))
                |> Map.ofSeq

            let batchedEvents = 
                counterToEventsLookup
                |> Map.keys 
                |> Seq.map (fun k -> 
                    {
                        CounterId = k
                        TimestampUtcEpochMs = counterToSmallestTimeLookup[k]
                        Hits = counterToIncrementSums[k]
                    })
                |> List.ofSeq

            batchedEvents
                
            

        let persistIncrementEventAsync (incrementEvent : IncrementCounterCommandEvent) = 
            async {
                let! dbConnection = createDbConnectionAsync()
                do! incrementCounterIOAsync dbConnection incrementEvent.CounterId incrementEvent.TimestampUtcEpochMs incrementEvent.Hits
            }

        let flushBatchWriterQueue messageList = 
            async {
                do! (batchIncrementEvents messageList
                |> Seq.map persistIncrementEventAsync
                |> Async.Sequential
                |> Async.Ignore)
            }

        let sendToBatchWriter = createBatchWriter batchSize flushBatchWriterQueue

        member this.Add (event : IncrementCounterCommandEvent) = sendToBatchWriter event