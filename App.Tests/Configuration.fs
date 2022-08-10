namespace AppTests


open Microsoft.Extensions.Configuration
open System.IO

open CloudSeedApp.Configuration

module Configuration =

// [<CLIMutable>]
// type TestConfiguration = {
//     DATABASE_HOST: string 
//     DATABASE_NAME: string 
//     DATABASE_PASSWORD: string 
//     DATABASE_USER: string 
//     IDONOTEXIST: string 
// }

    let fetchConfiguration = lazy (
        let configurationRoot = ((ConfigurationBuilder())
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.Test.json", false)
                            .Build())

        let root = configurationRoot.Get<AppConfiguration>()
        printfn "Root: %A" root
        root)