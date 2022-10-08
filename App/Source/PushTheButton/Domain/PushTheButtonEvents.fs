namespace CloudSeedApp

module PushTheButtonEvents = 

    let PUSH_THE_BUTTON_COUNTER_NAME = "push_the_button"

    type PushTheButtonCommand = {
        TimestampUtcEpochMs: int64
        Hits: int64
    }

    type PushTheButtonCommandErrors = 
        | FailedToPersist of string 
        | ValidationTooManyHits of string
        | ValidationHitsNegative of string

    type GetPushTotalsQuery = {
        StartTimeUtcEpochMs: int64
    }

    // GetButtonPushesTotalQuery

    type GetButtonPushesTotalQueryEvent = {
        StartTimeUtcEpochMs: int64 
        EndTimeUtcEpochMs: int64
    }

    type GetButtonPushesTotalQueryResultOk = int64 

    type GetButtonPushesTotalQueryResultError = string

    type GetButtonPushesTotalQueryResult = Async<Result<GetButtonPushesTotalQueryResultOk, GetButtonPushesTotalQueryResultError>>
