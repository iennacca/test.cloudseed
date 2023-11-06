namespace CloudSeedApp

open Microsoft.EntityFrameworkCore
open Events
open FSharp.Control
open Giraffe
open SentinelDomain
open System.Linq
open System.Threading.Tasks

open SentinelServiceTree


module SentinelQueries =

    let sendGetSentinelsQueryAsync 
        (serviceTree : SentinelServiceTree) 
        (event : GetSentinelsQuery) 
        : Async<Result<seq<Sentinel>, GetSentinelsQueryErrors>> 
        = 
        async {
            use dbContext = serviceTree.DbContext()

            let! sentinels = 
                dbContext.Sentinels.ToListAsync()
                |> Async.AwaitTask          

            return Ok sentinels
        } 

    let sendGetSentinelQueryAsync 
        (serviceTree : SentinelServiceTree) 
        (event : GetSentinelQuery) 
        : Async<Result<Sentinel, GetSentinelQueryErrors>> 
        = 
        async {
            use db = serviceTree.DbContext()

            let! sentinel = 
                db
                    .Sentinels
                    .Where(fun s -> s.id = event.id)
                    .Select(fun s -> Some s)
                    .SingleOrDefaultAsync()
                |> Async.AwaitTask

            return 
                match sentinel with
                | Some x -> Ok x 
                | None -> Error GetSentinelQueryErrors.NoSentinelFound
        } 
