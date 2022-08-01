// namespace CloudSeedApp

open System
open Microsoft
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open CloudSeedApp.Routes

let webApp: HttpFunc -> AspNetCore.Http.HttpContext -> HttpFuncResult =
    routes 

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

    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe webApp

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

