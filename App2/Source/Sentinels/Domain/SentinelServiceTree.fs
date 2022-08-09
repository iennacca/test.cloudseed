namespace CloudSeedApp

open System.Data.Common

module SentinelServiceTree = 

    type SentinelWorkflowIOs = {
        DbConnection: unit -> DbConnection
    }

    type SentinelServiceTree = {
        WorkflowIOs : SentinelWorkflowIOs
    }