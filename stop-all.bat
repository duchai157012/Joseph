@echo off
echo Stopping Infrastructure (Docker containers)...
docker-compose down
echo.
echo ========================================================
echo   Infrastructure Stopped!
echo.
echo   IMPORTANT: 
echo   Please manually CLOSE all the opened console windows 
echo   running the Microservices (Catalog, Order, etc.)
echo ========================================================
pause
