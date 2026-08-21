# ---- Etapa 1: build (imagen con SDK) ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AppGastos.Api/AppGastos.Api.csproj AppGastos.Api/
RUN dotnet restore AppGastos.Api/AppGastos.Api.csproj

COPY . .
RUN dotnet publish AppGastos.Api/AppGastos.Api.csproj -c Release -o /app/publish

# ---- Etapa 2: runtime (imagen mínima, sin SDK) ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AppGastos.Api.dll"]