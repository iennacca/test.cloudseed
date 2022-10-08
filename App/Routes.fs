namespace CloudSeedApp

open Microsoft
open Microsoft.AspNetCore.Http
open System
open System.Data.Common

open Configuration
open Events
open GetSentinelQuery
open Giraffe

open CloudSeedApp.Persistence
open CloudSeedApp.ServiceTree
open CounterServiceTree
open GetSentinelsQuery
open GetButtonPushesTotalQuery
open GetCounterTotalQuery
open IncrementCounterBatchWriter
open IncrementCounterCommand
open Persistence
open PushTheButtonCommand
open Sentinel
open SimpleTimedMemoryCache
open WebResponse

module Routes =

    type AppEvent =
        | SentinelEvent of SentinelEvents.SentinelEvent
        | FakeEvent of int

    let buildServiceTree (appConfiguration: AppConfiguration) (connectionString : string) : ServiceTree = 
        let dbConnectionAsync = fun() -> getDbConnectionAsync connectionString

        let incrementCounterBatchWriter = new IncrementCounterBatchWriter(100, dbConnectionAsync)
        
        let counterServiceTree = {
            CounterReadCache = (createTimeBasedCacheAsync 2000 100)
            DbConnectionAsync = dbConnectionAsync
            IncrementCounterBatchWriter = incrementCounterBatchWriter.Add
            UtcNowEpochMs = fun() -> DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        }

        {
            Settings = {
                AppConfiguration = appConfiguration
            }
            CounterServiceTree = counterServiceTree
            PushTheButtonServiceTree = {
                DbConnectionAsync = dbConnectionAsync
                GetCounterTotalQuery = sendGetCounterTotalQueryAsync counterServiceTree // GetCounterTotalQueryEvent -> Async<Result<int64, string>>
                SendIncrementCounterCommand = sendIncrementCounterCommandAsync counterServiceTree // IncrementCounterCommandEvent -> unit
                UtcNowEpochMs = fun() -> DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            }
            SentinelServiceTree = {
                DbConnectionAsync = dbConnectionAsync
            }
        }

    let apiResult<'TSuccess,'TError> (handler : HttpFunc -> HttpContext -> Async<Result<'TSuccess, 'TError>>): HttpHandler = 
        fun(next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! result = handler next ctx
                return! match result with 
                | Ok x -> json (x : WebResponse.WebResponseSuccess<'TSuccess>) next ctx
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
                            route "/" >=> 
                                warbler (fun _ -> 
                                    apiResult (
                                        (getSentinelsQueryHttpHandler serviceTree.SentinelServiceTree)
                                    ))
                            route "/ping"   >=> text "pong"
                            route "/sentinels" >=> 
                                warbler (fun _ -> 
                                    apiResult (
                                        (getSentinelsQueryHttpHandler serviceTree.SentinelServiceTree)
                                    ))
                        ]
                ]
            )
            subRoute "/push-the-button" (
                choose[
                    GET >=> 
                        choose [
                            route "/push/totals" >=> 
                                warbler (fun _ -> 
                                    apiResult (
                                        (createGetButtonPushesTotalQueryHttpHandler serviceTree.PushTheButtonServiceTree)
                                    ))
                        ]
                    POST >=> 
                        choose [
                            route "/push" >=> 
                                warbler (fun _ -> 
                                    apiResult (
                                        (createPushTheButtonCommandHttpHandler serviceTree.PushTheButtonServiceTree)
                                    ))
                        ]
                ]
            )
            route "/"       >=> htmlFile "/pages/index.html" ]