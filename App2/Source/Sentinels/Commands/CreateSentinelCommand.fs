namespace CloudSeedApp

open Microsoft.AspNetCore.Http
open Events
open FSharp.Control
open Giraffe
open Sentinel
open System
open System.Threading.Tasks

open SentinelDataRepository
open SentinelEvents
open SentinelServiceTree


module CreateSentinelCommand =

    let sendCreateSentinelCommandAsync (serviceTree : SentinelServiceTree) : Async<Result<Sentinel, SentinelEvents.CreateSentinelCommandErrors>> = 
        async {
            let dbConnection = serviceTree.WorkflowIOs.DbConnection()

            let newSentinel : Sentinel = {
                id = Guid.NewGuid().ToString()
                data = {
                    name = Guid.NewGuid().ToString()
                }
            }

            let! sentinels = createSentinelIOAsync dbConnection newSentinel           

            return match sentinels with
                | Some x -> Ok x 
                | None -> Error SentinelEvents.CreateSentinelCommandErrors.CouldNotCreateSentinel
        } 

    let createSentinelCommandHttpHandler (sentinelServiceTree : SentinelServiceTree) = 
        fun(next : HttpFunc) (ctx : HttpContext) -> 
            task {
                let sentinelResult = (sendCreateSentinelCommandAsync sentinelServiceTree)
                return! json sentinelResult next ctx
            }
