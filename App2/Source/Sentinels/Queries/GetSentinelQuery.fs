namespace CloudSeedApp

open Microsoft.AspNetCore.Http
open Events
open Giraffe
open Sentinel
open System.Threading.Tasks


module GetSentinelQuery =

    type GetSentinelQueryErrors =
        | NoEventReceived
        | NoSentinelFound

    type GetSentinelQuery = {
        id: string
    }

    let sendGetSentinelQueryAsync (event : GetSentinelQuery) : Task<Result<Sentinel, GetSentinelQueryErrors>> = 
        (Ok ({ id = "iamanid" }: Sentinel))
        |> Task.FromResult

    let getSentinelQueryHttpHandler = 
        fun(next : HttpFunc) (ctx : HttpContext) -> 
            task {
                let sentinelResult = (sendGetSentinelQueryAsync { id = "iamasentinelid" })
                return! json sentinelResult next ctx
            }
