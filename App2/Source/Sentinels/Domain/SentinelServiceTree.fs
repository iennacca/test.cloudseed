namespace CloudSeedApp

module SentinelServiceTree = 

    type SentinelWorkflows = {
        GetSentinelQuery: SentinelEvents.GetSentinelQuery -> Sentinel.Sentinel 
    }

    type SentinelWorkflowIOs = {
        NotAnIO: string -> unit
    }