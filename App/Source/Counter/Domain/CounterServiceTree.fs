namespace CloudSeedApp

open System
open System.Data.Common

open CounterEvents
open CounterHourly

module CounterServiceTree = 

    type CounterServiceTree = {
        CounterReadCache: int64 -> string -> Async<CounterHourly option> -> Async<CounterHourly option>
        DbConnectionAsync: unit -> Async<DbConnection>
        IncrementCounterBatchWriter: IncrementCounterCommandEvent -> unit
        UtcNowEpochMs: unit -> int64
    }