namespace CloudSeedApp

open Microsoft.AspNetCore.Http
open Events
open FSharp.Control
open Giraffe
open Sentinel
open System.Threading.Tasks

open CreateSentinelCommand
open SentinelDataRepository
open SentinelEvents
open SentinelServiceTree


module GetSentinelsQuery =

    let sendGetSentinelsQueryAsync (serviceTree : SentinelServiceTree) (event : GetSentinelsQuery) : Async<Result<seq<Sentinel>, SentinelEvents.GetSentinelsQueryErrors>> = 
        async {
            use! dbConnection = serviceTree.DbConnectionAsync()

            let! createdSentinel = sendCreateSentinelCommandAsync serviceTree
            let! sentinels = (getSentinelsIOAsync dbConnection event.count)            

            return Ok sentinels
        } 

    let getSentinelsQueryHttpHandler (sentinelServiceTree : SentinelServiceTree) = 
        fun(next : HttpFunc) (ctx : HttpContext) -> 
            async {
                let! sentinelResult = (sendGetSentinelsQueryAsync sentinelServiceTree { count = 10 })
                return sentinelResult
            }
