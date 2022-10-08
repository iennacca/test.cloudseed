namespace CloudSeedApp

open System
open System.Data.Common

open CounterEvents

module PushTheButtonServiceTree = 
    type PushTheButtonServiceTree = {
        DbConnectionAsync: unit -> Async<DbConnection>
        GetCounterTotalQuery: GetCounterTotalQueryEvent -> Async<Result<int64, string>>
        SendIncrementCounterCommand: IncrementCounterCommandEvent -> Async<Result<bool,unit>>
        UtcNowEpochMs: unit -> int64
    }