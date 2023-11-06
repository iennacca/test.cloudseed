namespace CloudSeedApp

open Microsoft
open Microsoft.AspNetCore.Http
open System
open System.Data.Common

open Configuration
open Events
open Giraffe
open Giraffe.EndpointRouting

open CloudSeedApp.Persistence
open CloudSeedApp.ServiceTree
open CounterServiceTree
open GetButtonPushesTotalQuery
open GetCounterTotalQuery
open IncrementCounterBatchWriter
open IncrementCounterCommand
open Persistence
open PushTheButtonCommand
open SentinelDomain
open SentinelPersistence
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
        let dbConnectionAsync = fun() -> getDbConnectionAsync connectionString

        let incrementCounterBatchWriter = 
            new IncrementCounterBatchWriter(100, dbConnectionAsync)
        
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
                DbContext = fun () -> new SentinelDataContext(connectionString)
            }
        }

    let apiResult<'TSuccess,'TError> 
        (handler : HttpFunc -> HttpContext -> Async<Result<'TSuccess, 'TError>>)
        : HttpHandler 
        = 
        fun(next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! result = handler next ctx
                return! match result with 
                | Ok x -> json (x : WebResponse.WebResponseSuccess<'TSuccess>) next ctx
                | Error (x : 'TError) -> json ({
                        ErrorCode = x
                    } : WebResponse.WebResponseError<'TError>) next ctx
            }

    let routes (configuration : AppConfiguration) (connectionString : string) =
        let serviceTree = buildServiceTree configuration connectionString

        [
            subRoute "" [
                GET [
                    route "/" ( 
                        apiResult (
                            (getSentinelsQueryHttpHandler serviceTree.SentinelServiceTree)
                        ))
                    route "/ping" (text "pong")
                    route "/sentinels" ( 
                        apiResult (
                            (getSentinelsQueryHttpHandler serviceTree.SentinelServiceTree)
                        ))
                ]
            ]
            subRoute "/push-the-button" [
                GET [
                    route "/push/totals" ( 
                        apiResult (
                            (createGetButtonPushesTotalQueryHttpHandler serviceTree.PushTheButtonServiceTree)
                        ))
                ]
                POST [
                    route "/push" ( 
                        apiResult (
                            (createPushTheButtonCommandHttpHandler serviceTree.PushTheButtonServiceTree)
                        ))
                ]
            ]
        ]