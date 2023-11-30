// namespace CloudSeedApp

open System
open System.IO
open Microsoft
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection

open Dapper.FSharp
open Giraffe
open Giraffe.EndpointRouting

open CloudSeedApp.Configuration
open CloudSeedApp.Persistence
open CloudSeedApp.Routes

let configureApp (app : IApplicationBuilder) =
    Dapper.FSharp.OptionTypes.register()

    let environment_name = 
        match (Environment
        .GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")) with 
        | null -> ""
        | x -> x.ToLower()
    let configuration = fetchConfiguration environment_name
    let connectionString = (
        getDatabaseConnectionString 
            configuration.DATABASE_HOST
            configuration.DATABASE_NAME
            configuration.DATABASE_USER
            configuration.DATABASE_PASSWORD)

    upgradeDatabase connectionString
        |> ignore

    let endpointsList = routes configuration connectionString

    app
        .UseStaticFiles()
        .UseRouting()
        .UseCors(
            Action<_>
                (fun (b: Infrastructure.CorsPolicyBuilder) ->
                    // Put real allowed origins in here
                    b.AllowAnyHeader() |> ignore
                    b.AllowAnyMethod() |> ignore
                    b.AllowAnyOrigin() |> ignore)
        )
        .UseEndpoints(
            fun e ->
                e.MapGiraffeEndpoints(endpointsList)
        )
    |> ignore

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
                    .UseKestrel(fun c -> c.AddServerHeader <- false)
                    .Configure(configureApp)
                    .ConfigureServices(configureServices)
                    |> ignore)
        .Build()
        .Run()
    0

