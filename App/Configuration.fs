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

    let fetchConfiguration (environment_name : string) =
        let targetConfigurationFile = (
            if environment_name.ToLower() = "development" 
                then "appsettings.Development.json" 
                else "appsettings.json")

        let configurationRoot = (
            (ConfigurationBuilder())
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(targetConfigurationFile, false)
                .Build())

        let root = configurationRoot.Get<AppConfiguration>()
        root