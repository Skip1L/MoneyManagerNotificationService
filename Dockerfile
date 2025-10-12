# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MoneyManagerNotificationServiceAPI/MoneyManagerNotificationServiceAPI.sln", "."]
COPY ["MoneyManagerNotificationServiceAPI/MoneyManagerNotificationServiceAPI.csproj", "MoneyManagerNotificationServiceAPI/"]
COPY ["DTOs/DTOs.csproj", "DTOs/"]
COPY ["Persistence/Persistence.csproj", "Persistence/"]
COPY ["Services/Services.csproj", "Services/"]

RUN dotnet restore "MoneyManagerNotificationServiceAPI/MoneyManagerNotificationServiceAPI.csproj" --force

COPY . .
WORKDIR "/src/MoneyManagerNotificationServiceAPI"
RUN dotnet publish "MoneyManagerNotificationServiceAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && \
    apt-get install -y --no-install-recommends wget && \
    rm -rf /var/lib/apt/lists/*

# Generate self-signed certificate for HTTPS
RUN apt-get update && apt-get install -y openssl && \
    mkdir -p /https && \
    openssl req -x509 -newkey rsa:4096 -sha256 -days 3650 -nodes \
        -keyout /https/aspnetapp.key \
        -out /https/aspnetapp.crt \
        -subj "/CN=notification-service" \
        -addext "subjectAltName=DNS:notification-service,DNS:localhost,IP:127.0.0.1" && \
    openssl pkcs12 -export -out /https/aspnetapp.pfx \
        -inkey /https/aspnetapp.key \
        -in /https/aspnetapp.crt \
        -passout pass:password

COPY --from=build /app/publish .

HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:6002/health || exit 1

EXPOSE 6001  
EXPOSE 6002 

ENV ASPNETCORE_URLS=https://+:6001;http://+:6002
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=password
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "MoneyManagerNotificationServiceAPI.dll"]