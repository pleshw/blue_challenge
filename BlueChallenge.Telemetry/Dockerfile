# syntax=docker/dockerfile:1.7

ARG BUILD_CONFIGURATION=Release

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION
WORKDIR /src

COPY ["BlueChallenge.Telemetry/BlueChallenge.Telemetry.csproj", "BlueChallenge.Telemetry/"]
RUN dotnet restore "BlueChallenge.Telemetry/BlueChallenge.Telemetry.csproj"

COPY . .
WORKDIR /src/BlueChallenge.Telemetry
RUN dotnet publish "BlueChallenge.Telemetry.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BlueChallenge.Telemetry.dll"]
