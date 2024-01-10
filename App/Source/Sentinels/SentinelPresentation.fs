namespace CloudSeedApp 

open Giraffe 
open Giraffe.EndpointRouting
open Microsoft.AspNetCore.Http
open SentinelCommands
open SentinelDomain
open SentinelServiceTree
open SentinelQueries

module MainPageView = 
    open Giraffe.ViewEngine

    type SentinelTableComponentProps = 
        {
            Sentinels : Sentinel list
        }

    let renderSentinelTableComponent
        (props : SentinelTableComponentProps)
        : XmlNode
        = 
        let giraffeTemplate = 
            table [
                _class "table table-zebra"
            ] (List.concat [
                [
                    tr [
                        _class "text-lg"
                    ] [
                        th [] [
                            Text "ID"
                        ]
                        th [] [
                            Text "Data"
                        ]
                    ];
                ];
                (
                    props.Sentinels
                    |> List.map (
                        fun s -> 
                            tr [] [
                                td [] [ Text s.id ]
                                td [] [ Text s.data.name ]
                            ]
                    )
                )
            ])
        
        giraffeTemplate
        

    type MainPageProps = 
        {
            SentinelCount : int
        }

    let renderMainPageAsync 
        (serviceTree : SentinelServiceTree) 
        (props : MainPageProps) 
        : Async<XmlNode>
        = 
        async {
            // Create sentinel on every hit so we can see data changing
            do!
                sendCreateSentinelCommandAsync serviceTree
                |> Async.Ignore

            let! sentinelResult = 
                    sendGetSentinelsQueryAsync serviceTree { count = props.SentinelCount }

            let sentinels = 
                match sentinelResult with
                | Ok s -> 
                    s 
                    |> Seq.toList
                | Error s -> raise (System.SystemException("Failed to get Sentinels"))

            let giraffeTemplate = 
                html [] [
                    head [] [
                        meta [
                            _charset "UTF-8"
                        ] 
                        link [
                            _rel "stylesheet";
                            _href "/css/tailwind.css";
                        ];
                        link [
                            _rel "stylesheet";
                            _href "/css/app.css";
                        ];
                        script [
                            _src "https://unpkg.com/htmx.org@1.9.9";
                        ] [];
                        title [] [ Text "Sentinels Table" ]
                    ]
                    body [] [
                        main [] [
                            h1 [
                                _class "text-xl"
                            ] [ Text "Sentinels Table" ]
                            (renderSentinelTableComponent { Sentinels = sentinels })
                        ]
                    ]
                ]

            return giraffeTemplate
        }
        

    let mainPageHttpHandler 
        (sentinelServiceTree : SentinelServiceTree) 
        = 
            fun (next : HttpFunc) (ctx : HttpContext) -> 
                    async {
                        return! 
                            renderMainPageAsync
                                sentinelServiceTree
                                {
                                    SentinelCount = 10
                                }
                    }


module SentinelEndpoints = 
    open Giraffe.ViewEngine

    let renderView 
        (handler : HttpFunc -> HttpContext -> Async<XmlNode>)
        : HttpHandler 
        =
        fun(next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! result = handler next ctx

                let resultString =
                    result 
                    |> RenderView.AsString.htmlDocument

                return! htmlString resultString next ctx
            }
    
    let constructEndpoints (serviceTree : SentinelServiceTree) = 
        [
            GET [
                route "/sentinels" (
                    renderView (
                        MainPageView.mainPageHttpHandler serviceTree
                    )
                )
            ]
        ]