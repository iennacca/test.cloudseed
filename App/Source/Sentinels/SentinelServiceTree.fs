namespace CloudSeedApp

open System.Data.Common

open Persistence
open SentinelDomain
open SentinelPersistence

module SentinelServiceTree = 
    type SentinelServiceTree = {
        DbContext: unit -> SentinelDataContext
    }