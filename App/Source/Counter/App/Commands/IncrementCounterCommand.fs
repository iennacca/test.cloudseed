namespace CloudSeedApp

open Microsoft.AspNetCore.Http
open Events
open FSharp.Control
open Giraffe
open System
open System.Threading.Tasks

open CounterServiceTree
open CounterEvents
open CounterTimeUtils


module IncrementCounterCommand =

    let sendIncrementCounterCommandAsync (serviceTree : CounterServiceTree) (event : IncrementCounterCommandEvent) : Async<Result<bool, unit>> = 
        async {
            // ham - maybe validation first for railroading...

            let timestampUtcEpochMsHoursGranularity = getTimestampEpochMsToHourGranularity event.TimestampUtcEpochMs
            serviceTree.IncrementCounterBatchWriter {event with TimestampUtcEpochMs = timestampUtcEpochMsHoursGranularity}

            return Ok true
        } 
