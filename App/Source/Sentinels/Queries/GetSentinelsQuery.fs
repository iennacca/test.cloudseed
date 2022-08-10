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

    let sendGetSentinelsQueryAsync (serviceTree : SentinelServiceTree) (event : GetSentinelsQuery) : Task<Result<seq<Sentinel>, SentinelEvents.GetSentinelsQueryErrors>> = 
        let getSentinels = async {
            let dbConnection = serviceTree.WorkflowIOs.DbConnection()

            let! createdSentinel = sendCreateSentinelCommandAsync serviceTree
            let! sentinels = (getSentinelsIOAsync dbConnection event.count)            

            return Ok sentinels
        } 
        getSentinels
        |> Async.StartAsTask

    let getSentinelsQueryHttpHandler (sentinelServiceTree : SentinelServiceTree) = 
        fun(next : HttpFunc) (ctx : HttpContext) -> 
            task {
                let sentinelResult = (sendGetSentinelsQueryAsync sentinelServiceTree { count = 10 })
                return! json sentinelResult next ctx
            }
