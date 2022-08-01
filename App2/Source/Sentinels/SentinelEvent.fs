namespace CloudSeedApp

open GetSentinelQuery

module SentinelEvent =

    type SentinelEvent =
        | GetSentinelQueryEvent of GetSentinelQuery
