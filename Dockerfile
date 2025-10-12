# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file first for better layer caching
COPY ["MoneyManagerNotificationServiceAPI/MoneyManagerNotificationServiceAPI.sln", "."]

# Copy main project
COPY ["MoneyManagerNotificationServiceAPI/MoneyManagerNotificationServiceAPI.csproj", "MoneyManagerNotificationServiceAPI/"]

# Copy all other project files
COPY ["DTOs/DTOs.csproj", "DTOs/"]
COPY ["Persistence/Persistence.csproj", "Persistence/"]
COPY ["Services/Services.csproj", "Services/"]

# Restore dependencies with clean cache
RUN dotnet restore "MoneyManagerNotificationServiceAPI/MoneyManagerNotificationServiceAPI.csproj" --force

# Copy everything else
COPY . .

# Build and publish
WORKDIR "/src/MoneyManagerNotificationServiceAPI"
RUN dotnet publish "MoneyManagerNotificationServiceAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install wget for health checks
RUN apt-get update && \
    apt-get install -y --no-install-recommends wget && \
    rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=build /app/publish .

# Copy template files needed in runtime
COPY --from=build /src/Services/Helpers/EmailTemplates/ ./Services/Helpers/EmailTemplates/

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:6002/health || exit 1

# Expose ports
EXPOSE 6001
EXPOSE 6002

# Environment variables
ENV ASPNETCORE_URLS=http://+:6002;https://+:6001
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "MoneyManagerNotificationServiceAPI.dll"]