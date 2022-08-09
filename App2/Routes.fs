namespace CloudSeedApp

open Microsoft
open System.Data.Common

open Configuration
open Events
open GetSentinelQuery
open Giraffe

open CloudSeedApp.Persistence
open CloudSeedApp.ServiceTree
open GetSentinelsQuery

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
                                    getSentinelsQueryHttpHandler serviceTree.SentinelServiceTree)
                        ]
                ]
            )
            route "/"       >=> htmlFile "/pages/index.html" ]