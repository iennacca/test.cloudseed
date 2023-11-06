namespace CloudSeedApp

open Giraffe 
open Microsoft.AspNetCore.Http
open SentinelServiceTree
open SentinelCommands
open SentinelQueries

module SentinelWorkflows = 

    let getSentinelsQueryHttpHandler (sentinelServiceTree : SentinelServiceTree) = 
        fun(next : HttpFunc) (ctx : HttpContext) -> 
            async {
                let! sentinelResult = 
                    sendGetSentinelsQueryAsync sentinelServiceTree { count = 10 }

                do!
                    sendCreateSentinelCommandAsync sentinelServiceTree
                    |> Async.Ignore

                return sentinelResult
            }

    let createSentinelCommandHttpHandler 
        (sentinelServiceTree : SentinelServiceTree) 
        = 
        fun(next : HttpFunc) (ctx : HttpContext) -> 
            task {
                let! sentinelResult = 
                    sendCreateSentinelCommandAsync sentinelServiceTree
                return sentinelResult
            }