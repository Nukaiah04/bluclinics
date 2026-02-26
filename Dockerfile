# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything from the repo
COPY . .

# Use a recursive search to find the .csproj file wherever it is
RUN dotnet publish DemoApp.csproj -c Release -o /app/publish /p:UseAppHost=false
# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

# Based on your .csproj, the output is DemoApp.dll
ENTRYPOINT ["dotnet", "DemoApp.dll"]
