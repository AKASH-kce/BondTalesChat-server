# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BondTalesChat-Server/BondTalesChat-Server.csproj", "BondTalesChat-Server/"]
RUN dotnet restore "BondTalesChat-Server/BondTalesChat-Server.csproj"
COPY . .
WORKDIR "/src/BondTalesChat-Server"
RUN dotnet build "BondTalesChat-Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BondTalesChat-Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy SQL files to the container
COPY BondTalesChat-Server/BondTalesChat-Database/ ./BondTalesChat-Database/
ENTRYPOINT ["dotnet", "BondTalesChat-Server.dll"]
