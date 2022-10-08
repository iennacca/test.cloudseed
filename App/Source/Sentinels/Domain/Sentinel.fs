namespace CloudSeedApp

module Sentinel = 

    [<CLIMutable>]
    type SentinelData = {
        name: string
    }

    // Mutable for Dapper
    [<CLIMutable>]
    type Sentinel = {
        id: string
        data: SentinelData
    }