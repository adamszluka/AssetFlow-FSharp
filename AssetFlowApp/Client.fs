module AssetFlowApp.Client

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html

[<JavaScript; SPAEntryPoint>]
let Main () =
    div [
        attr.style "max-width: 900px; margin: 40px auto; padding: 24px; font-family: Arial, sans-serif;"
    ] [
        div [
            attr.style "background: linear-gradient(135deg, #37474f, #78909c); color: white; padding: 24px; border-radius: 16px; margin-bottom: 24px;"
        ] [
            h1 [ attr.style "margin: 0 0 8px 0;" ] [
                text "AssetFlow"
            ]

            p [ attr.style "margin: 0; font-size: 16px;" ] [
                text "IT asset management web application built with F# and WebSharper."
            ]
        ]

        div [
            attr.style "background: white; padding: 20px; border-radius: 14px; box-shadow: 0 1px 4px rgba(0,0,0,0.08);"
        ] [
            h2 [] [ text "Project Omega" ]

            p [] [
                text "This application will be used to track IT assets, owners, statuses, purchase years, and replacement needs."
            ]

            ul [] [
                li [] [ text "Add IT assets" ]
                li [] [ text "Track owners and purchase years" ]
                li [] [ text "Filter assets by type and status" ]
                li [] [ text "Calculate asset age and replacement status" ]
                li [] [ text "Display asset statistics" ]
            ]
        ]
    ]
    |> Doc.RunById "main"