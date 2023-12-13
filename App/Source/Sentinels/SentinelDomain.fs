namespace CloudSeedApp

module SentinelDomain = 

    [<CLIMutable>]
    type SentinelData = {
        name: string
    }

    // Mutable for EF
    [<CLIMutable>]
    type Sentinel = {
        id: string
        data: SentinelData
    }

    type GetSentinelQueryErrors =
        | NoEventReceived
        | NoSentinelFound

    type GetSentinelQuery = {
        id: string
    }

    type GetSentinelsQueryErrors =
        | NoSentinelFound

    type GetSentinelsQuery = {
        count: int
    }

    type CreateSentinelCommand = unit 

    type CreateSentinelCommandErrors = 
        | CouldNotCreateSentinel

    type SentinelEvent =
        | CreateSentinelCommandEvent of CreateSentinelCommand
        | GetSentinelQueryEvent of GetSentinelQuery
        | GetSentinelsQueryEvent of GetSentinelsQuery