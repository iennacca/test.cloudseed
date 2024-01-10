namespace CloudSeedApp

open System
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

open Giraffe 

open SentinelServiceTree
open SentinelCommands
open SentinelQueries

module SentinelWorkflows = 

    let getSentinelsQueryHttpHandler (sentinelServiceTree : SentinelServiceTree) = 
        fun(next : HttpFunc) (ctx : HttpContext) -> 
            async {
                let logger = ctx.GetLogger("SentinelWorkflows")
                logger.LogInformation("Testing logging feature")

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