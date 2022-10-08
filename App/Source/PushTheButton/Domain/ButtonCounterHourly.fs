namespace CloudSeedApp

module ButtonCounterHourly = 

    // Mutable for Dapper
    [<CLIMutable>]
    type ButtonCounterHourly = {
        EpochTimeMs: int64
        TotalHits: int64
    }