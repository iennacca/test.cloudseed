namespace CloudSeedApp

open Microsoft
open Microsoft.AspNetCore.Http
open System.Data.Common

open Configuration
open Events
open GetSentinelQuery
open Giraffe

open CloudSeedApp.Persistence
open CloudSeedApp.ServiceTree
open GetSentinelsQuery
open WebResponse

module Routes =

    type AppEvent =
        | SentinelEvent of SentinelEvents.SentinelEvent
        | FakeEvent of int

    let buildServiceTree (appConfiguration: AppConfiguration) (connectionString : string) : ServiceTree = 
        let dbConnection = fun() -> getDatabaseConnection connectionString
        {
            Settings = {
                AppConfiguration = appConfiguration
            }
            SentinelServiceTree = {
                // Workflows = {
                //     GetSentinelQuery = sendGetSentinelQueryAsync 
                // }
                WorkflowIOs = {
                    DbConnection = dbConnection
                }
            }
            WorkflowIOs = {
                NotAnIO = 1
                getDbConnection = dbConnection
            }
            Workflows = {
                NotAWorkflow = 1
            }
        }

    let apiResult<'TSuccess,'TError> (fn : Async<Result<'TSuccess, 'TError>>): HttpHandler = 
        fun(next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! result = fn
                return! match result with 
                | Ok x -> json ({ 
                        Payload = x
                    } : WebResponse.WebResponseSuccess<'TSuccess>) next ctx
                | Error (x : 'TError) -> json ({
                        ErrorCode = x
                    } : WebResponse.WebResponseError<'TError>) next ctx
            }

    let routes (configuration : AppConfiguration) (connectionString : string) : HttpFunc -> AspNetCore.Http.HttpContext -> HttpFuncResult =
        let serviceTree = buildServiceTree configuration connectionString

        choose [
            subRoute "" (
                choose[
                    GET >=> 
                        choose [
                            route "/ping"   >=> text "pong"
                            route "/sentinels" >=> 
                                warbler (fun _ -> 
                                    apiResult (
                                        sendGetSentinelsQueryAsync serviceTree.SentinelServiceTree { count = 10 }
                                    ))
                        ]
                ]
            )
            route "/"       >=> htmlFile "/pages/index.html" ]