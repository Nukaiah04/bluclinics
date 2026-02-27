# -------- Build Stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj first (better caching)
COPY *.csproj ./
RUN dotnet restore

# Copy remaining files
COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# -------- Runtime Stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "BluClinicsApi.dll"]
