@echo off
echo Seeding Database...
docker cp init-data.sql joseph-sqlserver:/tmp/init-data.sql
docker exec -i joseph-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Password123! -i /tmp/init-data.sql
echo.
echo ========================================================
echo   Database Seeded!
echo ========================================================
pause
