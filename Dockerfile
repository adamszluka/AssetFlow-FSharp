FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

RUN apt-get update && apt-get install -y nodejs npm

COPY . ./

RUN dotnet restore ./AssetFlowApp/AssetFlowApp.fsproj
RUN dotnet publish ./AssetFlowApp/AssetFlowApp.fsproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/out ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "AssetFlowApp.dll"]