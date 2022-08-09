// namespace CloudSeedApp

open System
open System.IO
open Microsoft
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection

open Dapper.FSharp
open Giraffe
open Npgsql

open CloudSeedApp.Configuration
open CloudSeedApp.Persistence
open CloudSeedApp.Routes

let webApp (configuration : AppConfiguration) (connectionString: string) : HttpFunc -> AspNetCore.Http.HttpContext -> HttpFuncResult =
    routes configuration connectionString

let configureApp (app : IApplicationBuilder) =
    app.UseCors(
        Action<_>
            (fun (b: Infrastructure.CorsPolicyBuilder) ->
                // Put real allowed origins in here
                b.AllowAnyHeader() |> ignore
                b.AllowAnyMethod() |> ignore
                b.AllowAnyOrigin() |> ignore)
    )
    |> ignore

    Dapper.FSharp.OptionTypes.register()

    let configuration = fetchConfiguration
    let connectionString = (getDatabaseConnectionString 
        configuration.DATABASE_HOST
        configuration.DATABASE_NAME
        configuration.DATABASE_USER
        configuration.DATABASE_PASSWORD)

    printfn "ConnectionString: %A: " connectionString

    upgradeDatabase connectionString
        |> ignore

    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe (webApp configuration connectionString)

let configureServices (services : IServiceCollection) =
    services.AddCors() |> ignore

    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main _ =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .Configure(configureApp)
                    .ConfigureServices(configureServices)
                    |> ignore)
        .Build()
        .Run()
    0

