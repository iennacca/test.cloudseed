namespace CloudSeedApp

open Microsoft
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open System
open System.Data.Common

open Configuration
open Events
open Giraffe
open Giraffe.EndpointRouting

open CloudSeedApp.Persistence
open CloudSeedApp.ServiceTree
open Persistence
open SentinelDomain
open SentinelPersistence
open SentinelEndpoints
open SentinelWorkflows
open SimpleTimedMemoryCache
open WebResponse


module Routes =

    type AppEvent =
        | SentinelEvent of SentinelEvent
        | FakeEvent of int

    let buildServiceTree 
        (appConfiguration: AppConfiguration) 
        (connectionString : string) 
        : ServiceTree 
        = 

        {
            Settings = {
                AppConfiguration = appConfiguration
            }
            SentinelServiceTree = {
                DbContext = fun () -> new SentinelDataContext(connectionString)
            }
        }

    let apiResult<'TSuccess,'TError> 
        (handler : HttpFunc -> HttpContext -> Async<Result<'TSuccess, 'TError>>)
        : HttpHandler 
        = 
        fun(next : HttpFunc) (ctx : HttpContext) ->
            task {

                let logger = ctx.GetLogger("Routes")
                logger.LogInformation("Testing logging feature")

                let! result = handler next ctx
                return! 
                    match result with 
                    | Ok x -> json (x : WebResponse.WebResponseSuccess<'TSuccess>) next ctx
                    | Error (x : 'TError) -> json ({
                            ErrorCode = x
                        } : WebResponse.WebResponseError<'TError>) next ctx
            }

    let routes (configuration : AppConfiguration) (connectionString : string) =
        let serviceTree = buildServiceTree configuration connectionString

        List.concat [
            [
                subRoute "" [
                    GET [
                        route "/api" ( 
                            apiResult (
                                (getSentinelsQueryHttpHandler serviceTree.SentinelServiceTree)
                            ))
                        route "/ping" (text "pong")
                        route "/html" (htmlFile "./wwwroot/pages/index.html") 
                    ]
                ]
            ];
            (SentinelEndpoints.constructEndpoints serviceTree.SentinelServiceTree);
        ]