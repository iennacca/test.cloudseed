open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe

let webApp =
    choose [
        route "/ping"   >=> text "pong"
        route "/"       >=> htmlFile "/pages/index.html" ]

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

// open System
// open Microsoft.AspNetCore.Builder
// open Microsoft.Extensions.Hosting
// open Microsoft.Extensions.DependencyInjection
// open Microsoft.Extensions.Logging
// open Microsoft.AspNetCore.Cors

// let configureApp (app: IApplicationBuilder) =
//     // app.UseGiraffeErrorHandler errorHandler |> ignore
//     app.UsePathBase("/api") |> ignore
//     app.UseAuthentication() |> ignore

//     app.UseCors(
//         Action<_>
//             (fun (b: Infrastructure.CorsPolicyBuilder) ->
//                 b.AllowAnyHeader() |> ignore
//                 b.AllowAnyMethod() |> ignore
//                 b.AllowAnyOrigin() |> ignore)
//     )
//     |> ignore
//     // app
//     // app.UseGiraffe webApp

// [<EntryPoint>]
// let main args =
//     let builder = WebApplication.CreateBuilder(args)
//     let app = builder.Build()

//     app.MapGet("/", Func<string>(fun () -> "Hello World!")) |> ignore

//     app.Run()

//     0 // Exit code

//     // Host.CreateDefaultBuilder()
//     //     .ConfigureWebHostDefaults(
//     //         fun webHostBuilder -> 
//     //             webHostBuilder
//     //                 .ConfigureAppConfiguration()
//     //             |> ignore
//     //     ).Build()
//     //     .Run()
//     // 0

//     // Host
//     //     .CreateDefaultBuilder()
//     //     .ConfigureWebHostDefaults(fun webHostBuilder ->
//     //         webHostBuilder
//     //             .ConfigureAppConfiguration(configureApp)
//     //             .ConfigureServices(configureServices)
//     //             .ConfigureLogging(configureLogging)
//     //         |> ignore)
//     //     .Build()
//     //     .Run()

//     // 0

