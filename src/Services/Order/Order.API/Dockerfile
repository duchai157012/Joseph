# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/Services/Order/Order.API/Order.API.csproj", "Services/Order/Order.API/"]
COPY ["src/Services/Order/Order.Application/Order.Application.csproj", "Services/Order/Order.Application/"]
COPY ["src/Services/Order/Order.Domain/Order.Domain.csproj", "Services/Order/Order.Domain/"]
COPY ["src/Services/Order/Order.Infrastructure/Order.Infrastructure.csproj", "Services/Order/Order.Infrastructure/"]
COPY ["src/BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]

# Restore dependencies
RUN dotnet restore "Services/Order/Order.API/Order.API.csproj"

# Copy source code
COPY src/ .

# Build and publish
WORKDIR "/src/Services/Order/Order.API"
RUN dotnet publish "Order.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

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

ENTRYPOINT ["dotnet", "Order.API.dll"]
