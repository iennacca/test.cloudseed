namespace CloudSeedApp

open System
open System.Data.Common
open System.Threading.Tasks
open FSharp.Control
open System.Text.Json

open Dapper
open Dapper.FSharp
open Dapper.FSharp.PostgreSQL

open CloudSeedApp.Sentinel

module SentinelDataRepository = 

    [<CLIMutable>]
    type SentinelPersist = {
        id: string 
        data: string
    }

    let sentinelTable = table'<SentinelPersist> "sentinels"

    let mapSentinelToSentinelPersist (sentinel : Sentinel) : SentinelPersist = 
        {
            id = sentinel.id
            data = JsonSerializer.Serialize sentinel.data
        }

    let mapSentinelPersistToSentinel (sentinelPersist : SentinelPersist) : Sentinel = 
        {
            id = sentinelPersist.id
            data = JsonSerializer.Deserialize<SentinelData> sentinelPersist.data
        }

    let createSentinelIOAsync (dbConnection : DbConnection) (sentinel : Sentinel) : Async<Sentinel option> =
        async {
            let sentinelPersist = mapSentinelToSentinelPersist sentinel

            let sql = "INSERT INTO sentinels 
                (id, data)
                VALUES
                (@id, @data ::jsonb)"
            do! dbConnection.ExecuteAsync(sql, sentinelPersist) 
                |> Async.AwaitTask
                |> Async.Ignore

            return Some sentinel
        }

    let getSentinelByIdIOAsync (dbConnection : DbConnection) (id : string) : Async<Sentinel option> =  
        async {
            let! sentinelPersists = 
                select {
                    for s in sentinelTable do 
                    where (s.id = id)
                } |> dbConnection.SelectAsync<SentinelPersist>
                |> Async.AwaitTask


            return sentinelPersists
                |> Seq.map mapSentinelPersistToSentinel 
                |> Seq.tryHead
        }

    let getSentinelsIOAsync (dbConnection : DbConnection) (takeCount : int) : Async<seq<Sentinel>> =  
        async {
            let sqlParameters = {|
                take = takeCount
            |}
            let sql = "SELECT id, data
                FROM sentinels
                ORDER BY id
                LIMIT @take"
            let! sentinelPersists = (dbConnection.QueryAsync<SentinelPersist>(sql, sqlParameters)
                |> Async.AwaitTask)


            return sentinelPersists
                |> Seq.map mapSentinelPersistToSentinel

        }
        
