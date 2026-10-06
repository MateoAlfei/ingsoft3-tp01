# ---- Etapa 1: build (imagen con SDK) ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Primero solo los archivos de proyecto, para aprovechar el cache de capas
COPY AppGastos.sln .
COPY AppGastos.Api/AppGastos.Api.csproj AppGastos.Api/
COPY AppGastos.Api.Tests/AppGastos.Api.Tests.csproj AppGastos.Api.Tests/
RUN dotnet restore AppGastos.sln

COPY . .
RUN dotnet publish AppGastos.Api/AppGastos.Api.csproj -c Release -o /app/publish

# ---- Etapa 2: tests (parte del build, con el SDK adentro) ----
FROM build AS test
ENTRYPOINT ["dotnet", "test", "AppGastos.sln", \
            "--logger", "trx;LogFileName=tests.trx", \
            "--results-directory", "/out"]

# ---- Etapa 3: runtime (imagen mínima, sin SDK ni tests) ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AppGastos.Api.dll"]