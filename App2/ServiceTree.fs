namespace CloudSeedApp

open System.Data.Common

open Dapper.FSharp

open Configuration
open SentinelServiceTree

module ServiceTree = 

    type Settings = {
        AppConfiguration: Configuration.AppConfiguration
    }
    type Workflows = {
        NotAWorkflow: int
    }
    type WorkflowIOs = {
        getDbConnection: unit -> DbConnection
        NotAnIO: int
    }

    type ServiceTree = {
        Settings: Settings 
        SentinelServiceTree: SentinelServiceTree
        Workflows: Workflows 
        WorkflowIOs: WorkflowIOs
    }