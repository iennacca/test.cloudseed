namespace CloudSeedApp

open Microsoft.AspNetCore.Http
open Events
open Giraffe
open Sentinel
open System.Threading.Tasks


module GetSentinelQuery =

    let sendGetSentinelQueryAsync (event : SentinelEvents.GetSentinelQuery) : Task<Result<Sentinel, SentinelEvents.GetSentinelQueryErrors>> = 
        (Ok ({ id = "iamanid" }: Sentinel))
        |> Task.FromResult

    let getSentinelQueryHttpHandler = 
        fun(next : HttpFunc) (ctx : HttpContext) -> 
            task {
                let sentinelResult = (sendGetSentinelQueryAsync { id = "iamasentinelid" })
                return! json sentinelResult next ctx
            }
