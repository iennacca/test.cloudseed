namespace CloudSeedApp

open Microsoft.AspNetCore.Http
open Events
open FSharp.Control
open Giraffe
open System
open System.Threading.Tasks

open ButtonCounterHourly
open PushTheButtonServiceTree
open PushTheButtonEvents

module PushTheButtonCommand =

    let asyncBind (next : 'a -> Async<Result<'b, 'c>>) (asyncResult : Async<Result<'a, 'c>>) = 
        async {
            let! result = asyncResult
            match result with
            | Error e -> return Error e
            | Ok x -> return! next x
        }

    let (>>=) asyncResult next = asyncBind next asyncResult

    let _validatePushTheButtonCommandAsync (event : PushTheButtonCommand) : Async<Result<PushTheButtonCommand, PushTheButtonCommandErrors>> =
        async {
            match event with 
                | e when e.Hits < 0 ->
                    return Error (PushTheButtonCommandErrors.ValidationHitsNegative "Hits are negative")
                | e when e.Hits > 100 ->
                    return Error (PushTheButtonCommandErrors.ValidationTooManyHits "too many hits")
                | _ -> return Ok event
        }

    let _pushTheButtonCommandAsync (serviceTree : PushTheButtonServiceTree) (event : PushTheButtonCommand) : Async<Result<bool, PushTheButtonCommandErrors>> =
         async {
            let! _ = serviceTree.SendIncrementCounterCommand {
                CounterId = PUSH_THE_BUTTON_COUNTER_NAME
                TimestampUtcEpochMs = serviceTree.UtcNowEpochMs()
                Hits = event.Hits
            }

            return Ok true
        }

    let sendPushTheButtonCommandAsync (serviceTree : PushTheButtonServiceTree) (event : PushTheButtonCommand) : Async<Result<bool, PushTheButtonCommandErrors>> = 
        async {
            let! result = (
                (_validatePushTheButtonCommandAsync event)
                >>= (_pushTheButtonCommandAsync serviceTree)
            )
            return match result with 
                | Error e -> Error e 
                | Ok _ -> Ok true
        } 

    [<CLIMutable>]
    type RawPushTheButtonCommandHttpPayload =
        {
            Hits : int
        }

    let createPushTheButtonCommandHttpHandler (serviceTree : PushTheButtonServiceTree) = 
        fun(next : HttpFunc) (ctx : HttpContext) -> 
            async {
                let! rawHttpPayload = (ctx.BindJsonAsync<RawPushTheButtonCommandHttpPayload>()
                    |> Async.AwaitTask)

                // let effectivePayloadHits = 
                //     match rawHttpPayload with 
                //     | Some r ->
                //         match r.Hits with 
                //         | Some h ->
                //             h
                //         | _ -> 1
                //     | _ -> 1
                let effectivePayloadHits = 
                    match box rawHttpPayload with 
                    | null -> 1
                    | _ -> 
                        match box rawHttpPayload.Hits with 
                        | null -> 1
                        | _ -> rawHttpPayload.Hits

                let event : PushTheButtonCommand = {
                    TimestampUtcEpochMs = serviceTree.UtcNowEpochMs()
                    Hits = int64 effectivePayloadHits
                }

                let! result = (sendPushTheButtonCommandAsync serviceTree event)
                return result
            }
