module Configuration 

open Microsoft.Extensions.Configuration
open System.IO

[<CLIMutable>]
type TestConfiguration = {
    DATABASE_HOST: string 
    DATABASE_NAME: string 
    DATABASE_PASSWORD: string 
    DATABASE_USER: string 
    IDONOTEXIST: string 
}

let fetchConfiguration =
    let configurationRoot = ((ConfigurationBuilder())
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false)
                        .Build())

    let root = configurationRoot.Get<TestConfiguration>()
    printfn "Root: %A" root
    root