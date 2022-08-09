namespace CloudSeedApp

open System
open System.Data.Common
open System.Threading.Tasks
open FSharp.Control

open Dapper
open Dapper.FSharp
open Dapper.FSharp.PostgreSQL

open CloudSeedApp.Sentinel

module SentinelDataRepository = 

    let sentinelTable = table'<Sentinel> "sentinels"

    let createSentinelIOAsync (dbConnection : DbConnection) (sentinel : Sentinel) : Async<Sentinel option> =
        async {
            insert {
                into sentinelTable
                value sentinel
            } |> dbConnection.InsertAsync<Sentinel>
            |> Async.AwaitTask
            |> ignore

            return Some sentinel
        }

    let getSentinelByIdIOAsync (dbConnection : DbConnection) (id : string) : Async<Sentinel option> =  
        async {
            let! sentinels = 
                select {
                    for s in sentinelTable do 
                    where (s.id = id)
                } |> dbConnection.SelectAsync<Sentinel>
                |> Async.AwaitTask

            return sentinels 
                |> Seq.tryHead
        }

    let getSentinelsIOAsync (dbConnection : DbConnection) (takeCount : int) : Async<seq<Sentinel>> =  
        async {
            let! sentinels = 
                select {
                    for s in sentinelTable do 
                    orderBy s.id
                    take takeCount
                } |> dbConnection.SelectAsync<Sentinel>
                |> Async.AwaitTask

            return sentinels 
        }
        
