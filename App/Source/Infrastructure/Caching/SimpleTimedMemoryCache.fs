namespace CloudSeedApp

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Linq

module SimpleTimedMemoryCache =

    type TimeCachedItem<'Item> = {
        Item: 'Item
        ExpiryTimeEpochMs: int64 }

    let createTimeBasedCacheAsync<'TCache> 
        (timeWindowMs : int64) 
        (totalItemCapacity : int)
        : (int64 -> string -> Async<'TCache> -> Async<'TCache>) 
        = 
        let cachedItemLookup = new ConcurrentDictionary<string, TimeCachedItem<'TCache>>()

        let trashCollect 
            (cachedItemLookup : ConcurrentDictionary<string, TimeCachedItem<'TCache>>) 
            (countToRemove : int) 
            : unit 
            = 
            match cachedItemLookup.Count with
            | count when count > totalItemCapacity ->
                cachedItemLookup.Keys 
                |> Seq.truncate countToRemove
                |> Seq.toList 
                |> Seq.iter (fun k -> 
                    cachedItemLookup.TryRemove(k) |> ignore)
                |> ignore
            | _ -> ()

        let fetchFromCacheOrFetcherAsync 
            (currentTimeMs : int64) 
            cacheId 
            (fetcherFnAsync : Async<'TCache>) 
            : Async<'TCache> 
            =
            async {
                let existingCachedItem = 
                    match cachedItemLookup.TryGetValue(cacheId) with 
                    | true, item ->
                        match item with 
                        | i when item.ExpiryTimeEpochMs > currentTimeMs ->
                            Some item 
                        | _ -> None
                    | _ -> None

                match existingCachedItem with 
                | None -> 
                    let! newItem = fetcherFnAsync
                    let newCachedItem = {
                        Item = newItem 
                        ExpiryTimeEpochMs = currentTimeMs + timeWindowMs
                    }
                    cachedItemLookup[cacheId] <- newCachedItem
                    trashCollect cachedItemLookup 2
                    return newItem
                | Some(cachedItem) -> 
                    return cachedItem.Item}

        fun (currentTimeMs : int64) cacheId (fetcherFnAsync : Async<'TCache>) -> 
            async {
                return! fetchFromCacheOrFetcherAsync currentTimeMs cacheId fetcherFnAsync
            }

    let createTimeBasedMemoAsync<'TMemo> (timeWindowMs : int64) =
        let placeholderCacheKey = "_memo_key"
        let cacheFunction = createTimeBasedCacheAsync<'TMemo> timeWindowMs 1

        fun (currentTimeMs : int64) (fetcherFnAsync : Async<'TMemo>) -> 
            cacheFunction currentTimeMs placeholderCacheKey fetcherFnAsync