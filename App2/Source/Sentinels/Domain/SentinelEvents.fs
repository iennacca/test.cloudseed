namespace CloudSeedApp

module SentinelEvents = 

    type GetSentinelQueryErrors =
        | NoEventReceived
        | NoSentinelFound

    type GetSentinelQuery = {
        id: string
    }

    type SentinelEvent =
        | GetSentinelQueryEvent of GetSentinelQuery