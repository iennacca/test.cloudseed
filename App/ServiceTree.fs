namespace CloudSeedApp

open System.Data.Common

open Dapper.FSharp

open Configuration
open SentinelServiceTree

module ServiceTree = 

    type Settings = {
        AppConfiguration: Configuration.AppConfiguration
    }

    type ServiceTree = {
        Settings: Settings 
        SentinelServiceTree: SentinelServiceTree
    }