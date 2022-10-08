namespace CloudSeedApp

module CounterHourly = 

    // Mutable for Dapper
    [<CLIMutable>]
    type CounterHourly = {
        CounterId: string
        TimestampEpochMs: int64
        TotalHits: int64
    }