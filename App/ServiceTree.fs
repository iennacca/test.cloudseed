namespace CloudSeedApp

open System.Data.Common

open Dapper.FSharp

open Configuration
open CounterServiceTree
open PushTheButtonServiceTree
open SentinelServiceTree

module ServiceTree = 

    type Settings = {
        AppConfiguration: Configuration.AppConfiguration
    }

    type ServiceTree = {
        Settings: Settings 
        CounterServiceTree: CounterServiceTree
        SentinelServiceTree: SentinelServiceTree
        PushTheButtonServiceTree: PushTheButtonServiceTree
    }