# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BondTalesChat-Server.csproj", "."]
RUN dotnet restore "./BondTalesChat-Server.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "BondTalesChat-Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BondTalesChat-Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BondTalesChat-Server.dll"]
