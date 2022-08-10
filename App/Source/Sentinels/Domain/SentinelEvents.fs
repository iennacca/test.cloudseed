namespace CloudSeedApp

module SentinelEvents = 

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