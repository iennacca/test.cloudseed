namespace CloudSeedApp

open System.Data.Common

open Persistence
open Sentinel

module SentinelServiceTree = 
    type SentinelServiceTree = {
        DbConnectionAsync: unit -> Async<DbConnection>
    }