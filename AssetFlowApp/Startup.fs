open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open WebSharper.AspNetCore
open AssetFlowApp

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    
    builder.Services.AddWebSharper()
        .AddAuthentication("WebSharper")
        .AddCookie("WebSharper", fun options -> ())
    |> ignore

    let app = builder.Build()

    if not (app.Environment.IsDevelopment()) then
        app.UseExceptionHandler("/Error")
            .UseHsts()
        |> ignore
    
    app.UseHttpsRedirection()
        .UseDefaultFiles()
        .UseStaticFiles()
    |> ignore

    app.Urls.Add("http://0.0.0.0:8080")

    app.Run()

    0