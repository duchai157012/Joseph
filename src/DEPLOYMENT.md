# Deployment Guide - Joseph Microservices

This document outlines the deployment requirements, environment variables, and setup instructions for the Joseph microservices application.

## Prerequisites

### Required Infrastructure

1. **SQL Server** (or Azure SQL Database)
   - Version: SQL Server 2019+ or Azure SQL Database
   - Required for: Auth.API, Catalog.API, Order.API

2. **RabbitMQ** (or Azure Service Bus)
   - Version: RabbitMQ 3.12+
   - Required for: Order.API (event-driven communication)

3. **Seq (Optional)** - Centralized Logging
   - Version: Latest
   - Port: 5341
   - Docker command: `docker run --name seq -d -p 5341:80 -e ACCEPT_EULA=Y datalust/seq:latest`

### .NET Runtime

- .NET 9.0 Runtime & SDK
- Download from: https://dotnet.microsoft.com/download/dotnet/9.0

---

## Environment Variables

### Auth.API

| Variable | Description | Example |
|----------|-------------|---------|
| `DB_CONNECTION_STRING` | SQL Server connection string | `Server=myserver.database.windows.net;Database=AuthDb;User Id=sa;Password=***;` |
| `JWT_SECRET_KEY` | Secret key for JWT token generation (min 32 characters) | `your-super-secret-jwt-key-change-this-in-production` |
| `JWT_ISSUER` | Token issuer identifier | `https://auth.yourdomain.com` |
| `JWT_AUDIENCE` | Token audience identifier | `https://api.yourdomain.com` |

### Catalog.API

| Variable | Description | Example |
|----------|-------------|---------|
| `DB_CONNECTION_STRING` | SQL Server connection string | `Server=myserver.database.windows.net;Database=CatalogDb;User Id=sa;Password=***;` |

### Order.API

| Variable | Description | Example |
|----------|-------------|---------|
| `DB_CONNECTION_STRING` | SQL Server connection string | `Server=myserver.database.windows.net;Database=OrderDb;User Id=sa;Password=***;` |
| `RABBITMQ_HOST` | RabbitMQ host address | `rabbitmq.yourdomain.com` |

---

## Configuration Files

### Production Configuration

Each API has an `appsettings.Production.json` file with environment variable placeholders:

- `${DB_CONNECTION_STRING}` - Will be replaced with actual connection string
- `${JWT_SECRET_KEY}` - Will be replaced with actual JWT secret
- `${RABBITMQ_HOST}` - Will be replaced with actual RabbitMQ host

### Setting Environment Variables

#### Windows (PowerShell)
```powershell
$env:DB_CONNECTION_STRING="Server=..."
$env:JWT_SECRET_KEY="your-secret-key"
```

#### Linux/macOS (Bash)
```bash
export DB_CONNECTION_STRING="Server=..."
export JWT_SECRET_KEY="your-secret-key"
```

#### Docker Compose
```yaml
environment:
  - DB_CONNECTION_STRING=Server=...
  - JWT_SECRET_KEY=your-secret-key
```

#### Azure App Service
- Navigate to: Configuration > Application Settings
- Add each environment variable as a new application setting

---

## Database Setup

### 1. Create Databases

```sql
CREATE DATABASE AuthDb;
CREATE DATABASE CatalogDb;
CREATE DATABASE OrderDb;
```

### 2. Run Migrations (if using EF Core Migrations)

```bash
# Auth.API
cd src/Services/Auth/Auth.Infrastructure
dotnet ef database update --startup-project ../Auth.API

# Catalog.API
cd src/Services/Catalog/Catalog.Infrastructure
dotnet ef database update --startup-project ../Catalog.API

# Order.API
cd src/Services/Order/Order.Infrastructure
dotnet ef database update --startup-project ../Order.API
```

**Note**: Currently, the APIs use `EnsureCreated()` which will auto-create databases on first run. Consider switching to EF Migrations for production.

---

## Running Locally

### 1. Start Infrastructure Services

```bash
# Start SQL Server (Docker)
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password123!" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest

# Start RabbitMQ (Docker)
docker run -d --hostname rabbitmq --name rabbitmq \
  -p 5672:5672 -p 15672:15672 \
  rabbitmq:3-management

# Start Seq (Optional - for centralized logging)
docker run --name seq -d -p 5341:80 \
  -e ACCEPT_EULA=Y datalust/seq:latest
```

### 2. Run APIs

```bash
# Terminal 1 - Auth.API
cd src/Services/Auth/Auth.API
dotnet run

# Terminal 2 - Catalog.API
cd src/Services/Catalog/Catalog.API
dotnet run

# Terminal 3 - Order.API
cd src/Services/Order/Order.API
dotnet run
```

### 3. Access Services

- **Auth.API**: http://localhost:5001
  - Swagger: http://localhost:5001/swagger
  - Health: http://localhost:5001/health

- **Catalog.API**: http://localhost:5002
  - Swagger: http://localhost:5002/swagger
  - Health: http://localhost:5002/health

- **Order.API**: http://localhost:5003
  - Swagger: http://localhost:5003/swagger
  - Health: http://localhost:5003/health

- **Seq (Logs)**: http://localhost:5341

---

## Health Checks

All APIs expose health check endpoints at `/health`:

```bash
curl http://localhost:5001/health  # Auth.API
curl http://localhost:5002/health  # Catalog.API
curl http://localhost:5003/health  # Order.API
```

Health checks verify:
- SQL Server connectivity
- RabbitMQ connectivity (Order.API only)
- Application startup status

---

## Logging

### Structured Logging with Serilog

All APIs use Serilog with three sinks:

1. **Console** - Structured output with timestamps
2. **File** - Rolling daily log files in `logs/` directory
   - Auth: `logs/auth-YYYYMMDD.txt`
   - Catalog: `logs/catalog-YYYYMMDD.txt`
   - Order: `logs/order-YYYYMMDD.txt`
3. **Seq** - Centralized log aggregation at http://localhost:5341

### Correlation IDs

All requests automatically include `X-Correlation-ID` header for distributed tracing:
- Automatically generated if not provided
- Propagated across service calls
- Included in all log entries

---

## Security Checklist

Before deploying to production:

- [ ] **Change all secrets**: JWT keys, database passwords, RabbitMQ credentials
- [ ] **Use strong passwords**: Minimum 32 characters for JWT secrets
- [ ] **Enable HTTPS**: Update `RequireHttpsMetadata = true` in JWT configuration
- [ ] **Use Azure Key Vault**: Store secrets in Key Vault, not appsettings
- [ ] **Restrict CORS**: Update `AllowedHosts` and CORS policies
- [ ] **Enable authentication**: Ensure all endpoints require authentication
- [ ] **Review logs**: Set appropriate log levels (Warning/Error for production)
- [ ] **Network security**: Use private endpoints, firewall rules

---

## Troubleshooting

### Common Issues

**1. Database Connection Failed**
- Verify SQL Server is running
- Check connection string format and credentials
- Ensure firewall allows port 1433

**2. RabbitMQ Connection Failed**
- Verify RabbitMQ is running
- Check RabbitMQ management UI: http://localhost:15672
- Ensure firewall allows port 5672

**3. JWT Authentication Failed**
- Verify JWT secret key is at least 32 characters
- Check Issuer and Audience match between Auth.API and other services
- Ensure token hasn't expired

**4. Seq Not Receiving Logs**
- Seq is optional - application will work without it
- Verify Seq is running: http://localhost:5341
- Check Serilog configuration in appsettings.json

---

## Support

For issues or questions:
- Check logs in `logs/` directory or Seq
- Review health check endpoints
- Verify all environment variables are set correctly
- Ensure all infrastructure services (SQL, RabbitMQ) are running
