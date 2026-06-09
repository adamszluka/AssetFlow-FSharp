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
type StatusFilter =
    | AllStatuses
    | OnlyActive
    | OnlyInRepair
    | OnlyRetired
    | OnlyMissing

[<JavaScript>]
type TypeFilter =
    | AllTypes
    | OnlyLaptops
    | OnlyDesktops
    | OnlyServers
    | OnlyVirtualMachines
    | OnlyNetworkDevices
    | OnlyOther

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

[<JavaScript>]
let parseYear (value: string) =
    match System.Int32.TryParse(value) with
    | true, year -> Some year
    | false, _ -> None

[<JavaScript>]
let isValidYear year =
    year >= 2000 && year <= currentYear

[<JavaScript>]
let filterByStatus statusFilter assets =
    match statusFilter with
    | AllStatuses -> assets
    | OnlyActive -> assets |> List.filter (fun asset -> asset.Status = Active)
    | OnlyInRepair -> assets |> List.filter (fun asset -> asset.Status = InRepair)
    | OnlyRetired -> assets |> List.filter (fun asset -> asset.Status = Retired)
    | OnlyMissing -> assets |> List.filter (fun asset -> asset.Status = Missing)

[<JavaScript>]
let filterByType typeFilter assets =
    match typeFilter with
    | AllTypes -> assets
    | OnlyLaptops -> assets |> List.filter (fun asset -> asset.AssetType = Laptop)
    | OnlyDesktops -> assets |> List.filter (fun asset -> asset.AssetType = Desktop)
    | OnlyServers -> assets |> List.filter (fun asset -> asset.AssetType = Server)
    | OnlyVirtualMachines -> assets |> List.filter (fun asset -> asset.AssetType = VirtualMachine)
    | OnlyNetworkDevices -> assets |> List.filter (fun asset -> asset.AssetType = NetworkDevice)
    | OnlyOther -> assets |> List.filter (fun asset -> asset.AssetType = Other)

[<JavaScript>]
let filterBySearch (searchText: string) assets =
    let search = searchText.Trim().ToLower()

    if search = "" then
        assets
    else
        assets
        |> List.filter (fun asset ->
            asset.Name.ToLower().Contains(search)
            || asset.Owner.ToLower().Contains(search)
        )

[<JavaScript>]
let applyFilters statusFilter typeFilter searchText assets =
    assets
    |> filterByStatus statusFilter
    |> filterByType typeFilter
    |> filterBySearch searchText

[<JavaScript>]
let countByStatus status assets =
    assets
    |> List.filter (fun asset -> asset.Status = status)
    |> List.length

[<JavaScript>]
let replacementDueAssets assets =
    assets
    |> List.filter isReplacementDue

[<JavaScript; SPAEntryPoint>]
let Main () =

    let nameVar = Var.Create ""
    let ownerVar = Var.Create ""
    let purchaseYearVar = Var.Create ""
    let selectedTypeVar = Var.Create Laptop
    let selectedStatusVar = Var.Create Active

    let statusFilterVar = Var.Create AllStatuses
    let typeFilterVar = Var.Create AllTypes
    let searchVar = Var.Create ""

    let validationMessageVar = Var.Create ""
    let nextIdVar = Var.Create 5

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

    let addAsset () =
        let name = nameVar.Value.Trim()
        let owner = ownerVar.Value.Trim()
        let yearText = purchaseYearVar.Value.Trim()

        if name = "" then
            validationMessageVar.Value <- "Asset name is required."
        elif owner = "" then
            validationMessageVar.Value <- "Owner is required."
        else
            match parseYear yearText with
            | None ->
                validationMessageVar.Value <- "Purchase year must be a valid number."
            | Some year when not (isValidYear year) ->
                validationMessageVar.Value <- "Purchase year must be between 2000 and 2026."
            | Some year ->
                let newAsset =
                    {
                        Id = nextIdVar.Value
                        Name = name
                        Owner = owner
                        AssetType = selectedTypeVar.Value
                        Status = selectedStatusVar.Value
                        PurchaseYear = year
                    }

                assetsVar.Value <- assetsVar.Value @ [ newAsset ]
                nextIdVar.Value <- nextIdVar.Value + 1

                nameVar.Value <- ""
                ownerVar.Value <- ""
                purchaseYearVar.Value <- ""
                selectedTypeVar.Value <- Laptop
                selectedStatusVar.Value <- Active
                validationMessageVar.Value <- ""

    let updateAssetStatus assetId newStatus =
        assetsVar.Value <-
            assetsVar.Value
            |> List.map (fun asset ->
                if asset.Id = assetId then
                    { asset with Status = newStatus }
                else
                    asset
            )

    let deleteAsset assetId =
        assetsVar.Value <-
            assetsVar.Value
            |> List.filter (fun asset -> asset.Id <> assetId)

    let typeButton label value =
        selectedTypeVar.View
        |> Doc.BindView (fun selectedType ->
            button [
                on.click (fun _ _ -> selectedTypeVar.Value <- value)
                attr.style (
                    if selectedType = value then
                        "margin-right: 8px; margin-bottom: 8px; padding: 8px 12px; border-radius: 8px; border: none; background: " + assetTypeColor value + "; color: white; font-weight: bold; cursor: pointer;"
                    else
                        "margin-right: 8px; margin-bottom: 8px; padding: 8px 12px; border-radius: 8px; border: 1px solid #cccccc; background: white; cursor: pointer;"
                )
            ] [ text label ]
        )

    let statusButton label value =
        selectedStatusVar.View
        |> Doc.BindView (fun selectedStatus ->
            button [
                on.click (fun _ _ -> selectedStatusVar.Value <- value)
                attr.style (
                    if selectedStatus = value then
                        "margin-right: 8px; margin-bottom: 8px; padding: 8px 12px; border-radius: 8px; border: none; background: " + assetStatusColor value + "; color: white; font-weight: bold; cursor: pointer;"
                    else
                        "margin-right: 8px; margin-bottom: 8px; padding: 8px 12px; border-radius: 8px; border: 1px solid #cccccc; background: white; cursor: pointer;"
                )
            ] [ text label ]
        )

    let statusFilterButton label value =
        statusFilterVar.View
        |> Doc.BindView (fun currentFilter ->
            button [
                on.click (fun _ _ -> statusFilterVar.Value <- value)
                attr.style (
                    if currentFilter = value then
                        "margin-right: 8px; margin-bottom: 8px; padding: 8px 12px; border-radius: 8px; border: none; background: #37474f; color: white; font-weight: bold; cursor: pointer;"
                    else
                        "margin-right: 8px; margin-bottom: 8px; padding: 8px 12px; border-radius: 8px; border: 1px solid #cccccc; background: white; cursor: pointer;"
                )
            ] [ text label ]
        )

    let typeFilterButton label value =
        typeFilterVar.View
        |> Doc.BindView (fun currentFilter ->
            button [
                on.click (fun _ _ -> typeFilterVar.Value <- value)
                attr.style (
                    if currentFilter = value then
                        "margin-right: 8px; margin-bottom: 8px; padding: 8px 12px; border-radius: 8px; border: none; background: #00695c; color: white; font-weight: bold; cursor: pointer;"
                    else
                        "margin-right: 8px; margin-bottom: 8px; padding: 8px 12px; border-radius: 8px; border: 1px solid #cccccc; background: white; cursor: pointer;"
                )
            ] [ text label ]
        )

    let statsPanel =
        assetsVar.View
        |> View.Map (fun assets ->
            let total = List.length assets
            let active = countByStatus Active assets
            let inRepair = countByStatus InRepair assets
            let retired = countByStatus Retired assets
            let missing = countByStatus Missing assets
            let due = assets |> replacementDueAssets |> List.length

            div [
                attr.style "display: grid; grid-template-columns: repeat(6, 1fr); gap: 14px; margin-bottom: 24px;"
            ] [
                div [ attr.style "padding: 16px; background: #f5f5f5; border-radius: 12px;" ] [
                    div [ attr.style "font-size: 13px; color: #666;" ] [ text "Total" ]
                    div [ attr.style "font-size: 26px; font-weight: bold;" ] [ text (string total) ]
                ]

                div [ attr.style "padding: 16px; background: #e8f5e9; border-radius: 12px;" ] [
                    div [ attr.style "font-size: 13px; color: #666;" ] [ text "Active" ]
                    div [ attr.style "font-size: 26px; font-weight: bold;" ] [ text (string active) ]
                ]

                div [ attr.style "padding: 16px; background: #fff8e1; border-radius: 12px;" ] [
                    div [ attr.style "font-size: 13px; color: #666;" ] [ text "In Repair" ]
                    div [ attr.style "font-size: 26px; font-weight: bold;" ] [ text (string inRepair) ]
                ]

                div [ attr.style "padding: 16px; background: #eeeeee; border-radius: 12px;" ] [
                    div [ attr.style "font-size: 13px; color: #666;" ] [ text "Retired" ]
                    div [ attr.style "font-size: 26px; font-weight: bold;" ] [ text (string retired) ]
                ]

                div [ attr.style "padding: 16px; background: #fce4ec; border-radius: 12px;" ] [
                    div [ attr.style "font-size: 13px; color: #666;" ] [ text "Missing" ]
                    div [ attr.style "font-size: 26px; font-weight: bold;" ] [ text (string missing) ]
                ]

                div [ attr.style "padding: 16px; background: #ffebee; border-radius: 12px;" ] [
                    div [ attr.style "font-size: 13px; color: #666;" ] [ text "Replacement due" ]
                    div [ attr.style "font-size: 26px; font-weight: bold;" ] [ text (string due) ]
                ]
            ]
        )
        |> Doc.EmbedView

    let replacementReport =
        assetsVar.View
        |> Doc.BindView (fun assets ->
            let dueAssets = replacementDueAssets assets

            div [
                attr.style "background: white; padding: 22px; border-radius: 14px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); margin-bottom: 24px;"
            ] [
                h2 [ attr.style "margin-top: 0;" ] [
                    text "Replacement Report"
                ]

                p [ attr.style "color: #555;" ] [
                    text "Assets are marked as replacement due when they are at least 5 years old."
                ]

                if List.isEmpty dueAssets then
                    div [
                        attr.style "padding: 14px; border-radius: 10px; background: #e8f5e9; color: #2e7d32; font-weight: bold;"
                    ] [
                        text "No assets are currently due for replacement."
                    ]
                else
                    div [] [
                        dueAssets
                        |> List.map (fun asset ->
                            div [
                                attr.style "padding: 12px; border-radius: 10px; background: #ffebee; margin-bottom: 10px; border-left: 5px solid #c62828;"
                            ] [
                                div [ attr.style "font-weight: bold;" ] [
                                    text asset.Name
                                ]

                                div [ attr.style "color: #555;" ] [
                                    text ("Owner: " + asset.Owner + " | Age: " + string (assetAge asset) + " years | Status: " + assetStatusToString asset.Status)
                                ]
                            ]
                        )
                        |> Doc.Concat
                    ]
            ]
        )

    let validationMessage =
        validationMessageVar.View
        |> Doc.BindView (fun message ->
            if message = "" then
                Doc.Empty
            else
                div [
                    attr.style "margin-top: 10px; padding: 10px; border-radius: 8px; background: #ffebee; color: #c62828; font-weight: bold;"
                ] [
                    text message
                ]
        )

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

                div [
                    attr.style "display: flex; flex-direction: column; gap: 8px; min-width: 140px;"
                ] [
                    button [
                        on.click (fun _ _ -> updateAssetStatus asset.Id Active)
                        attr.style "padding: 8px 10px; border-radius: 8px; border: none; background: #2e7d32; color: white; cursor: pointer;"
                    ] [ text "Set Active" ]

                    button [
                        on.click (fun _ _ -> updateAssetStatus asset.Id InRepair)
                        attr.style "padding: 8px 10px; border-radius: 8px; border: none; background: #f9a825; color: white; cursor: pointer;"
                    ] [ text "Set In Repair" ]

                    button [
                        on.click (fun _ _ -> updateAssetStatus asset.Id Retired)
                        attr.style "padding: 8px 10px; border-radius: 8px; border: none; background: #616161; color: white; cursor: pointer;"
                    ] [ text "Retire" ]

                    button [
                        on.click (fun _ _ -> updateAssetStatus asset.Id Missing)
                        attr.style "padding: 8px 10px; border-radius: 8px; border: none; background: #c62828; color: white; cursor: pointer;"
                    ] [ text "Mark Missing" ]

                    button [
                        on.click (fun _ _ -> deleteAsset asset.Id)
                        attr.style "padding: 8px 10px; border-radius: 8px; border: none; background: #b71c1c; color: white; font-weight: bold; cursor: pointer;"
                    ] [ text "Delete" ]
                ]
            ]
        ]

    let assetList =
        let filtersView =
            View.Map2
                (fun statusFilter typeFilter -> statusFilter, typeFilter)
                statusFilterVar.View
                typeFilterVar.View

        let combinedFilterView =
            View.Map2
                (fun filters searchText -> filters, searchText)
                filtersView
                searchVar.View

        View.Map2
            (fun assets (filters, searchText) -> assets, filters, searchText)
            assetsVar.View
            combinedFilterView
        |> Doc.BindView (fun (assets, (statusFilter, typeFilter), searchText) ->
            let filteredAssets =
                assets
                |> applyFilters statusFilter typeFilter searchText

            if List.isEmpty filteredAssets then
                div [
                    attr.style "padding: 18px; border-radius: 12px; background: #fafafa; color: #666;"
                ] [
                    text "No assets match the selected filters."
                ]
            else
                filteredAssets
                |> List.map assetCard
                |> Doc.Concat
        )

    div [
        attr.style "max-width: 1000px; margin: 40px auto; padding: 24px; font-family: Arial, sans-serif; background: #fcfcfc;"
    ] [
        div [
            attr.style "background: linear-gradient(135deg, #37474f, #78909c); color: white; padding: 28px; border-radius: 16px; margin-bottom: 24px;"
        ] [
            h1 [ attr.style "margin: 0 0 8px 0; font-size: 34px;" ] [ text "AssetFlow" ]
            p [ attr.style "margin: 0; font-size: 16px;" ] [
                text "IT asset management web application built with F# and WebSharper."
            ]
        ]

        statsPanel
        replacementReport

        div [
            attr.style "background: white; padding: 22px; border-radius: 14px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); margin-bottom: 24px;"
        ] [
            h2 [ attr.style "margin-top: 0;" ] [ text "Add new asset" ]

            div [ attr.style "margin-bottom: 10px;" ] [
                Doc.InputType.Text [
                    attr.placeholder "Asset name"
                    attr.style "width: 100%; padding: 10px; border-radius: 8px; border: 1px solid #cccccc; box-sizing: border-box;"
                ] nameVar
            ]

            div [ attr.style "margin-bottom: 10px;" ] [
                Doc.InputType.Text [
                    attr.placeholder "Owner"
                    attr.style "width: 100%; padding: 10px; border-radius: 8px; border: 1px solid #cccccc; box-sizing: border-box;"
                ] ownerVar
            ]

            div [ attr.style "margin-bottom: 14px;" ] [
                Doc.InputType.Text [
                    attr.placeholder "Purchase year"
                    attr.style "width: 100%; padding: 10px; border-radius: 8px; border: 1px solid #cccccc; box-sizing: border-box;"
                ] purchaseYearVar
            ]

            div [ attr.style "margin-bottom: 12px;" ] [
                div [ attr.style "font-weight: bold; margin-bottom: 6px;" ] [ text "Asset type" ]

                typeButton "Laptop" Laptop
                typeButton "Desktop" Desktop
                typeButton "Server" Server
                typeButton "VM" VirtualMachine
                typeButton "Network" NetworkDevice
                typeButton "Other" Other
            ]

            div [ attr.style "margin-bottom: 14px;" ] [
                div [ attr.style "font-weight: bold; margin-bottom: 6px;" ] [ text "Asset status" ]

                statusButton "Active" Active
                statusButton "In Repair" InRepair
                statusButton "Retired" Retired
                statusButton "Missing" Missing
            ]

            button [
                on.click (fun _ _ -> addAsset ())
                attr.style "padding: 10px 16px; border-radius: 8px; border: none; background: #37474f; color: white; font-weight: bold; cursor: pointer;"
            ] [ text "Add asset" ]

            validationMessage
        ]

        div [
            attr.style "background: white; padding: 22px; border-radius: 14px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); margin-bottom: 24px;"
        ] [
            h2 [ attr.style "margin-top: 0;" ] [ text "Filters" ]

            div [ attr.style "margin-bottom: 16px;" ] [
                div [ attr.style "font-weight: bold; margin-bottom: 8px;" ] [
                    text "Search"
                ]

                Doc.InputType.Text [
                    attr.placeholder "Search by asset name or owner"
                    attr.style "width: 100%; padding: 10px; border-radius: 8px; border: 1px solid #cccccc; box-sizing: border-box;"
                ] searchVar
            ]

            div [ attr.style "margin-bottom: 16px;" ] [
                div [ attr.style "font-weight: bold; margin-bottom: 8px;" ] [ text "Status filter" ]

                statusFilterButton "All" AllStatuses
                statusFilterButton "Active" OnlyActive
                statusFilterButton "In Repair" OnlyInRepair
                statusFilterButton "Retired" OnlyRetired
                statusFilterButton "Missing" OnlyMissing
            ]

            div [] [
                div [ attr.style "font-weight: bold; margin-bottom: 8px;" ] [ text "Type filter" ]

                typeFilterButton "All Types" AllTypes
                typeFilterButton "Laptop" OnlyLaptops
                typeFilterButton "Desktop" OnlyDesktops
                typeFilterButton "Server" OnlyServers
                typeFilterButton "VM" OnlyVirtualMachines
                typeFilterButton "Network" OnlyNetworkDevices
                typeFilterButton "Other" OnlyOther
            ]
        ]

        h2 [] [ text "Assets" ]

        div [] [
            assetList
        ]
    ]
    |> Doc.RunById "main"