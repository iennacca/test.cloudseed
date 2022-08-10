namespace CloudSeedApp

open System.IO
open Microsoft.Extensions.Configuration

module Configuration = 

    [<CLIMutable>]
    type AppConfiguration = {
        DATABASE_HOST: string 
        DATABASE_NAME: string 
        DATABASE_PASSWORD: string 
        DATABASE_USER: string 
        IDONOTEXIST: string 
    }

    let fetchConfiguration =
        // hamytodo: Probably want to have different Dev vs production settings
        let configurationRoot = ((ConfigurationBuilder())
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", false)
                            .Build())

        let root = configurationRoot.Get<AppConfiguration>()
        printfn "Root: %A" root
        root