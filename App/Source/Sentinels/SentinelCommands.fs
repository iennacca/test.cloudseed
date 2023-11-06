namespace CloudSeedApp

open Microsoft.AspNetCore.Http
open Events
open FSharp.Control
open Giraffe
open System
open System.Threading.Tasks

open Persistence

open SentinelDomain
open SentinelServiceTree


module SentinelCommands =

    let sendCreateSentinelCommandAsync 
        (serviceTree : SentinelServiceTree) 
        : Async<Result<Sentinel, CreateSentinelCommandErrors>> 
        = 
        async {
            use db = serviceTree.DbContext()

            let newSentinel : Sentinel = {
                id = Guid.NewGuid().ToString()
                data = {
                    name = Guid.NewGuid().ToString()
                }
            }

            db.Sentinels.Add(newSentinel)
                |> ignore

            let! linesChanged =
                db.SaveChangesAsync()
                |> Async.AwaitTask

            return 
                match linesChanged with
                | x when x = 1 -> Ok newSentinel 
                | _ -> Error CreateSentinelCommandErrors.CouldNotCreateSentinel

        } 

