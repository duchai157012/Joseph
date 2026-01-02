# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/Gateway/Gateway.API.csproj", "Gateway/"]

# Restore dependencies
RUN dotnet restore "Gateway/Gateway.API.csproj"

# Copy source code
COPY src/ .

# Build and publish
WORKDIR "/src/Gateway"
RUN dotnet publish "Gateway.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Create non-root user for security
RUN addgroup -g 1000 appuser && adduser -u 1000 -G appuser -s /bin/sh -D appuser
USER appuser

EXPOSE 8080
EXPOSE 8081

# Copy published app from build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Gateway.API.dll"]
