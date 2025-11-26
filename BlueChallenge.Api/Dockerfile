# syntax=docker/dockerfile:1.7

ARG BUILD_CONFIGURATION=Release

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
ARG BUILD_CONFIGURATION
WORKDIR /src

COPY ["BlueChallenge.Api/BlueChallenge.Api.csproj", "BlueChallenge.Api/"]
COPY ["BlueChallenge.Telemetry/BlueChallenge.Telemetry.csproj", "BlueChallenge.Telemetry/"]
RUN dotnet restore "BlueChallenge.Api/BlueChallenge.Api.csproj"

FROM restore AS publish
ARG BUILD_CONFIGURATION
COPY . .
WORKDIR /src/BlueChallenge.Api
RUN dotnet publish "BlueChallenge.Api.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlueChallenge.Api.dll"]
