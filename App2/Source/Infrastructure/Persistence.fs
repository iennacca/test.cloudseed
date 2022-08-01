namespace CloudSeedApp 

open System.Reflection
open System.Threading

open DbUp
open Npgsql

module Persistence =

    let getDatabaseConnectionString databaseHost databaseName databaseUser databasePassword  = 
        let builder = (new NpgsqlConnectionStringBuilder())
        builder.Host <- databaseHost
        builder.Database <- databaseName
        builder.Username <- databaseUser
        builder.Password <- databasePassword
        builder.SslMode <- SslMode.Disable
        builder.Pooling <- true

        builder.ToString()

    let upgradeDatabase connectionString =
        printfn "DB Connection String: %A" connectionString
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
                    scriptName.Contains("DatabaseUpgradeScripts.DatabaseUpgradeScript")
                ))
            .WithTransactionPerScript()
            .LogToConsole()
            .Build())

        let result = upgrader.PerformUpgrade()
        match result.Successful with 
        | false -> failwith "Failed to upgrade database"
        | true -> printfn "Database Upgrade Success!"

