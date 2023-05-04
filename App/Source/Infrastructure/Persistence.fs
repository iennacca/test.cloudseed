namespace CloudSeedApp 

open System.Data.Common
open System.Reflection
open System.Threading

open DbUp
open Npgsql

module Persistence =

    type IDBIOAsync<'a> = (DbConnection -> Async<'a>) -> Async<'a>

    let getDatabaseConnectionString databaseHost databaseName databaseUser databasePassword  = 
        let builder = (new NpgsqlConnectionStringBuilder())
        builder.Host <- databaseHost
        builder.Database <- databaseName
        builder.Username <- databaseUser
        builder.Password <- databasePassword
        builder.SslMode <- SslMode.Disable
        builder.Pooling <- true

        builder.ToString()

    let getDbConnectionAsync connectionString : Async<DbConnection> = 
        async {
            let dbConnection = new NpgsqlConnection(connectionString)
            return dbConnection
        }

    let getDatabaseConnection connectionString : DbConnection = 
        new NpgsqlConnection(connectionString)

    let withDbIoAsync connectionString (callbackAsync : DbConnection -> Async<'a>) = 
        async {
            use dbConnection = new NpgsqlConnection(connectionString)
            do! dbConnection.OpenAsync() |> Async.AwaitTask

            return! callbackAsync dbConnection
        }

    let upgradeDatabase connectionString =
        try 
            EnsureDatabase.For.PostgresqlDatabase(connectionString)
        with 
        | _ as e ->
            printfn "Could not connect to database - delaying before retry"
            Thread.Sleep(5000)
            EnsureDatabase.For.PostgresqlDatabase(connectionString)

        let upgrader : Engine.UpgradeEngine = (DeployChanges.To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                (fun (scriptName : string) -> 
                    // Console.WriteLine($"DBUp scriptName: {scriptName}");
                    scriptName.Contains("DatabaseUpgradeScripts.DBUP")
                ))
            .WithTransactionPerScript()
            .LogToConsole()
            .Build())

        let result = upgrader.PerformUpgrade()
        match result.Successful with 
        | false -> failwith "Failed to upgrade database!"
        | true -> printfn "Database Upgrade Success!"

