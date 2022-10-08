namespace CloudSeedApp

module CounterEvents = 

    type IncrementCounterCommandEvent = {
        CounterId: string
        TimestampUtcEpochMs: int64
        Hits: int64
    }

    type GetCounterTotalQueryEvent = {
        CounterId: string
        StartTimestampUtcEpochMs: int64
        EndTimestampUtcEpochMs: int64
    }