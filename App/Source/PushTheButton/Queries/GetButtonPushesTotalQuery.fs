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

module GetButtonPushesTotalQuery =

    let getButtonPushesTotalQueryAsync (serviceTree : PushTheButtonServiceTree) (event : GetButtonPushesTotalQueryEvent) : GetButtonPushesTotalQueryResult = 
        async {
            // ham - maybe validation first for railroading...

            let! counterTotal = serviceTree.GetCounterTotalQuery {
                CounterId = PUSH_THE_BUTTON_COUNTER_NAME
                StartTimestampUtcEpochMs = event.StartTimeUtcEpochMs
                EndTimestampUtcEpochMs = event.EndTimeUtcEpochMs
            }

            let buttonPusshesTotalResult = 
                match counterTotal with 
                    | Ok c -> Ok c
                    | Error e -> Error e

            return buttonPusshesTotalResult
        } 

    // [<CLIMutable>]
    // type GetButtonPushesTotalQueryHttpPayload =
    //     {
    //     }


    let createGetButtonPushesTotalQueryHttpHandler (serviceTree : PushTheButtonServiceTree) = 
        fun(next : HttpFunc) (ctx : HttpContext) -> 
            async {

                let event : GetButtonPushesTotalQueryEvent = {
                    StartTimeUtcEpochMs = 0L
                    EndTimeUtcEpochMs = serviceTree.UtcNowEpochMs()
                }

                let! result = (getButtonPushesTotalQueryAsync serviceTree event)
                return result
            }
