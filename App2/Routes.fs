namespace CloudSeedApp

open Microsoft
open Giraffe

module Routes =

    let routes: HttpFunc -> AspNetCore.Http.HttpContext -> HttpFuncResult =
        choose [
            subRoute "" (
                choose[
                    GET >=> 
                        choose [
                            route "/ping"   >=> text "pong"
                        ]
                ]
            )
            route "/"       >=> htmlFile "/pages/index.html" ]