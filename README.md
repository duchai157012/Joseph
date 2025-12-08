# Joseph Microservices System

## Overview
This is a comprehensive .NET 8 Microservices solution with a React Frontend.

## Architecture
- **Backend Services**: Clean Architecture (.NET 8 Web API)
    - **Auth**: Identity & Token management
    - **Catalog**: Product & Inventory
    - **Order**: Order processing
    - **Payment**: Payment processing
    - **Notification**: Email/SMS (Consumer)
- **Gateway**: YARP Reverse Proxy
- **Frontend**: React (Vite) + TypeScript
- **Infrastructure**: SQL Server, RabbitMQ, Redis, Seq (via Docker)

## Project Structure
```
src/
  Services/
    Auth/
    Catalog/
    Order/
    Payment/
    Notification/
  Gateway/
  BuildingBlocks/ (Shared Kernel - To Be Implemented)
Client/ (Frontend - To Be Initialized)
docker-compose.yml
```

## How to Run the Application

### Option 1: One-Click Script (Recommended)
We have provided a PowerShell script to start everything for you.
1. Open a terminal in the solution root.
2. Run:
   ```powershell
   ./start-all.ps1
   ```
   This will:
   - Start Docker Infrastructure (SQL, RabbitMQ, etc).
   - Open 6 new console windows for each Microservice + Gateway.

### Option 2: Visual Studio
1. Right-click the Solution `Joseph.sln` -> **Properties**.
2. Select **Multiple Startup Projects**.
3. Set the following to **Start**:
   - `Auth.API`
   - `Catalog.API`
   - `Order.API`
   - `Payment.API`
   - `Notification.API`
   - `Gateway.API`
4. Press **F5**.
5. Ensure `docker-compose up -d` is running in a terminal.

### Access Points
- **Gateway (Entry Point)**: [http://localhost:8080](http://localhost:8080)
- **RabbitMQ Dashboard**: [http://localhost:15672](http://localhost:15672) (guest/guest)
- **Seq Logs**: [http://localhost:8081](http://localhost:8081)

#### API Swagger UIs (Direct Access)
- **Catalog**: http://localhost:5000/swagger
- **Order**: http://localhost:5001/swagger
- **Auth**: http://localhost:5002/swagger
- **Payment**: http://localhost:5003/swagger
- **Notification**: http://localhost:5004/swagger
