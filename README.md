# AssetFlow

AssetFlow is an IT asset management web application built with F# and WebSharper.

The goal of the project is to help track IT devices, their owners, statuses, purchase years, and replacement needs in a simple and interactive way.

## Motivation

In an IT environment, it is important to keep track of devices, owners, statuses, and replacement cycles. AssetFlow was created as a simple web-based tool for managing IT assets and demonstrating functional programming concepts in F#.

## Planned Features

- Add IT assets
- Select asset type
- Select asset status
- Track owner and purchase year
- Update asset status
- Delete assets
- Filter by type and status
- Search by asset name or owner
- View statistics
- Detect assets that are due for replacement

## Technologies

- F#
- .NET 10
- WebSharper
- ASP.NET Core

## Functional programming aspects

The project will use several functional programming concepts:

- record types for asset data
- discriminated unions for asset type and status
- pattern matching
- immutable-style record updates
- list transformations with `List.map` and `List.filter`
- reactive state handling with `Var` and `View`

## Build and run locally

```bash
cd AssetFlowApp
dotnet build
dotnet run
```

## Then open:
http://localhost:5000

## Screenshot

-To be added.

##Live Demo

-To be added.