@echo off
echo Starting Infrastructure...
docker-compose up -d

echo Waiting for DB to warm up (10s)...
timeout /t 10

echo Starting Microservices...
start "Catalog API" dotnet run --project src/Services/Catalog/Catalog.API/Catalog.API.csproj --urls=http://LAPTOP-QAH6AS0H:5000
start "Order API" dotnet run --project src/Services/Order/Order.API/Order.API.csproj --urls=http://LAPTOP-QAH6AS0H:5001
start "Auth API" dotnet run --project src/Services/Auth/Auth.API/Auth.API.csproj --urls=http://LAPTOP-QAH6AS0H:5002
start "Payment API" dotnet run --project src/Services/Payment/Payment.API/Payment.API.csproj --urls=http://LAPTOP-QAH6AS0H:5003
start "Notification API" dotnet run --project src/Services/Notification/Notification.API/Notification.API.csproj --urls=http://LAPTOP-QAH6AS0H:5004
start "Gateway API" dotnet run --project src/Gateway/Gateway.API.csproj --urls=http://LAPTOP-QAH6AS0H:8080

echo.
echo ========================================================
echo   System Started! 
echo ========================================================
echo   1. Gateway:      http://LAPTOP-QAH6AS0H:8080
echo   2. Swagger (Auth): http://LAPTOP-QAH6AS0H:5002/swagger
echo   3. Swagger (Catalog): http://LAPTOP-QAH6AS0H:5000/swagger
echo   4. Swagger (Order):   http://LAPTOP-QAH6AS0H:5001/swagger
echo.
echo   NOTE: If services fail to connect to DB, ensure Docker is running!
echo ========================================================
pause
