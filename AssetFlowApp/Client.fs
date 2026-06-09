module AssetFlowApp.Client

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html

[<JavaScript>]
type AssetType =
    | Laptop
    | Desktop
    | Server
    | VirtualMachine
    | NetworkDevice
    | Other

[<JavaScript>]
type AssetStatus =
    | Active
    | InRepair
    | Retired
    | Missing

[<JavaScript>]
type Asset =
    {
        Id: int
        Name: string
        Owner: string
        AssetType: AssetType
        Status: AssetStatus
        PurchaseYear: int
    }

[<JavaScript>]
let assetTypeToString assetType =
    match assetType with
    | Laptop -> "Laptop"
    | Desktop -> "Desktop"
    | Server -> "Server"
    | VirtualMachine -> "Virtual Machine"
    | NetworkDevice -> "Network Device"
    | Other -> "Other"

[<JavaScript>]
let assetStatusToString status =
    match status with
    | Active -> "Active"
    | InRepair -> "In Repair"
    | Retired -> "Retired"
    | Missing -> "Missing"

[<JavaScript>]
let assetStatusColor status =
    match status with
    | Active -> "#2e7d32"
    | InRepair -> "#f9a825"
    | Retired -> "#616161"
    | Missing -> "#c62828"

[<JavaScript>]
let assetTypeColor assetType =
    match assetType with
    | Laptop -> "#1976d2"
    | Desktop -> "#7b1fa2"
    | Server -> "#455a64"
    | VirtualMachine -> "#00838f"
    | NetworkDevice -> "#ef6c00"
    | Other -> "#5d4037"

[<JavaScript>]
let currentYear = 2026

[<JavaScript>]
let assetAge asset =
    currentYear - asset.PurchaseYear

[<JavaScript>]
let isReplacementDue asset =
    assetAge asset >= 5

[<JavaScript>]
let replacementText asset =
    if isReplacementDue asset then
        "Replacement due"
    else
        "OK"

[<JavaScript>]
let replacementColor asset =
    if isReplacementDue asset then
        "#c62828"
    else
        "#2e7d32"

[<JavaScript; SPAEntryPoint>]
let Main () =

    let assetsVar =
        Var.Create [
            {
                Id = 1
                Name = "DELL Latitude 5420"
                Owner = "Adam Szluka"
                AssetType = Laptop
                Status = Active
                PurchaseYear = 2021
            }
            {
                Id = 2
                Name = "VM-WEB-01"
                Owner = "Infrastructure Team"
                AssetType = VirtualMachine
                Status = Active
                PurchaseYear = 2023
            }
            {
                Id = 3
                Name = "Juniper EX2300"
                Owner = "Network Team"
                AssetType = NetworkDevice
                Status = InRepair
                PurchaseYear = 2019
            }
            {
                Id = 4
                Name = "HP ProDesk 600"
                Owner = "Finance Department"
                AssetType = Desktop
                Status = Retired
                PurchaseYear = 2018
            }
        ]

    let statsPanel =
        assetsVar.View
        |> View.Map (fun assets ->
            let total = List.length assets
            let active =
                assets
                |> List.filter (fun a -> a.Status = Active)
                |> List.length

            let inRepair =
                assets
                |> List.filter (fun a -> a.Status = InRepair)
                |> List.length

            let replacementDue =
                assets
                |> List.filter isReplacementDue
                |> List.length

            div [
                attr.style "display: grid; grid-template-columns: repeat(4, 1fr); gap: 14px; margin-bottom: 24px;"
            ] [
                div [ attr.style "padding: 16px; background: #f5f5f5; border-radius: 12px;" ] [
                    div [ attr.style "font-size: 13px; color: #666;" ] [ text "Total assets" ]
                    div [ attr.style "font-size: 26px; font-weight: bold;" ] [ text (string total) ]
                ]

                div [ attr.style "padding: 16px; background: #e8f5e9; border-radius: 12px;" ] [
                    div [ attr.style "font-size: 13px; color: #666;" ] [ text "Active" ]
                    div [ attr.style "font-size: 26px; font-weight: bold;" ] [ text (string active) ]
                ]

                div [ attr.style "padding: 16px; background: #fff8e1; border-radius: 12px;" ] [
                    div [ attr.style "font-size: 13px; color: #666;" ] [ text "In repair" ]
                    div [ attr.style "font-size: 26px; font-weight: bold;" ] [ text (string inRepair) ]
                ]

                div [ attr.style "padding: 16px; background: #ffebee; border-radius: 12px;" ] [
                    div [ attr.style "font-size: 13px; color: #666;" ] [ text "Replacement due" ]
                    div [ attr.style "font-size: 26px; font-weight: bold;" ] [ text (string replacementDue) ]
                ]
            ]
        )
        |> Doc.EmbedView

    let assetCard asset =
        div [
            attr.style "padding: 18px; border: 1px solid #e0e0e0; border-radius: 14px; background: white; box-shadow: 0 1px 4px rgba(0,0,0,0.08); margin-bottom: 14px;"
        ] [
            div [
                attr.style "display: flex; justify-content: space-between; align-items: flex-start; gap: 16px; flex-wrap: wrap;"
            ] [
                div [] [
                    h3 [ attr.style "margin: 0 0 8px 0;" ] [
                        text asset.Name
                    ]

                    div [ attr.style "color: #555; margin-bottom: 8px;" ] [
                        text ("Owner: " + asset.Owner)
                    ]

                    div [ attr.style "color: #555; margin-bottom: 8px;" ] [
                        text ("Purchase year: " + string asset.PurchaseYear + " | Age: " + string (assetAge asset) + " years")
                    ]

                    span [
                        attr.style ("display: inline-block; margin-right: 8px; padding: 5px 10px; border-radius: 999px; color: white; font-size: 12px; font-weight: bold; background: " + assetTypeColor asset.AssetType + ";")
                    ] [
                        text (assetTypeToString asset.AssetType)
                    ]

                    span [
                        attr.style ("display: inline-block; margin-right: 8px; padding: 5px 10px; border-radius: 999px; color: white; font-size: 12px; font-weight: bold; background: " + assetStatusColor asset.Status + ";")
                    ] [
                        text (assetStatusToString asset.Status)
                    ]

                    span [
                        attr.style ("display: inline-block; padding: 5px 10px; border-radius: 999px; color: white; font-size: 12px; font-weight: bold; background: " + replacementColor asset + ";")
                    ] [
                        text (replacementText asset)
                    ]
                ]
            ]
        ]

    let assetList =
        assetsVar.View
        |> Doc.BindView (fun assets ->
            assets
            |> List.map assetCard
            |> Doc.Concat
        )

    div [
        attr.style "max-width: 1000px; margin: 40px auto; padding: 24px; font-family: Arial, sans-serif; background: #fcfcfc;"
    ] [
        div [
            attr.style "background: linear-gradient(135deg, #37474f, #78909c); color: white; padding: 28px; border-radius: 16px; margin-bottom: 24px;"
        ] [
            h1 [ attr.style "margin: 0 0 8px 0; font-size: 34px;" ] [
                text "AssetFlow"
            ]

            p [ attr.style "margin: 0; font-size: 16px;" ] [
                text "IT asset management web application built with F# and WebSharper."
            ]
        ]

        statsPanel

        div [
            attr.style "background: white; padding: 22px; border-radius: 14px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); margin-bottom: 24px;"
        ] [
            h2 [ attr.style "margin-top: 0;" ] [
                text "IT Asset Inventory"
            ]

            p [ attr.style "color: #555;" ] [
                text "This dashboard shows IT assets, owners, statuses, purchase years, asset age, and replacement status."
            ]
        ]

        h2 [] [
            text "Assets"
        ]

        div [] [
            assetList
        ]
    ]
    |> Doc.RunById "main"