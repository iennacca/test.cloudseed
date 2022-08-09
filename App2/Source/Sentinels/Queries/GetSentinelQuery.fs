namespace CloudSeedApp

open Microsoft.AspNetCore.Http
open Events
open FSharp.Control
open Giraffe
open Sentinel
open System.Threading.Tasks

open SentinelDataRepository
open SentinelEvents
open SentinelServiceTree


module GetSentinelQuery =

    let sendGetSentinelQueryAsync (serviceTree : SentinelServiceTree) (event : SentinelEvents.GetSentinelQuery) : Async<Result<Sentinel, SentinelEvents.GetSentinelQueryErrors>> = 
        async {
            let dbConnection = serviceTree.WorkflowIOs.DbConnection()
            let! sentinels = (getSentinelByIdIOAsync dbConnection event.id)            

            return match sentinels with
                | Some x -> Ok x 
                | None -> Error GetSentinelQueryErrors.NoSentinelFound
        } 

    let getSentinelQueryHttpHandler (sentinelServiceTree : SentinelServiceTree) = 
        fun(next : HttpFunc) (ctx : HttpContext) -> 
            task {
                let sentinelResult = (sendGetSentinelQueryAsync sentinelServiceTree { id = "iamasentinelid" })
                return! json sentinelResult next ctx
            }
