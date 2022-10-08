namespace CloudSeedApp

open System

module CounterTimeUtils = 

    let getTimestampEpochMsToHourGranularity (timestampUtcEpochMs : int64) = 
        let nowTime = DateTimeOffset.FromUnixTimeMilliseconds(timestampUtcEpochMs)

        let dateTimeoffsetToHours = (nowTime
            .AddMinutes(float (nowTime.Minute * (-1)))
            .AddSeconds(float (nowTime.Second * (-1)))
            .AddMilliseconds(float (nowTime.Millisecond * (-1))))
            
        dateTimeoffsetToHours.ToUnixTimeMilliseconds()
