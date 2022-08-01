namespace CloudSeedApp

open Microsoft

open Events
open GetSentinelQuery
open Giraffe
open SentinelEvent

module Routes =

    type AppEvent =
        | SentinelEvent of SentinelEvent
        | FakeEvent of int

    let routes : HttpFunc -> AspNetCore.Http.HttpContext -> HttpFuncResult =
        

        choose [
            subRoute "" (
                choose[
                    GET >=> 
                        choose [
                            route "/ping"   >=> text "pong"
                            route "/sentinels" >=> warbler (fun _ -> getSentinelQueryHttpHandler)
                        ]
                ]
            )
            route "/"       >=> htmlFile "/pages/index.html" ]