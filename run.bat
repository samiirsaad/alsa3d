@echo off
echo ==========================================
echo   Al-Sa3d Accounting System - Launcher
echo ==========================================
echo.

echo [1/4] Restoring NuGet Packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages. Please check your internet connection.
    pause
    exit /b 1
)

echo [2/4] Building the solution...
dotnet build --no-restore
if %errorlevel% neq 0 (
    echo ERROR: Build failed. Check compilation errors above.
    pause
    exit /b 1
)

echo [3/4] Setting up Database (Requires SQL Server)...
echo Checking for sqlcmd...
where sqlcmd >nul 2>nul
if %errorlevel% neq 0 (
    echo WARNING: sqlcmd not found. Skipping database setup.
    echo Please manually run the SQL scripts in the 'database' folder.
    goto RUN_APP
)

echo Executing SQL Scripts...
set DB_SERVER=localhost
set DB_NAME=AlSa3d

echo Creating Database...
sqlcmd -S %DB_SERVER% -E -i database\01_Create_Database.sql
echo Creating Tables...
sqlcmd -S %DB_SERVER% -E -i database\02_Create_Tables.sql
echo Seeding Data...
sqlcmd -S %DB_SERVER% -E -i database\03_Seed_Data.sql
echo Creating Stored Procedures...
sqlcmd -S %DB_SERVER% -E -i database\04_Stored_Procedures.sql
echo Creating Views & Functions...
sqlcmd -S %DB_SERVER% -E -i database\05_Views_Functions.sql

:RUN_APP
echo [4/4] Launching Al-Sa3d Application...
cd src\AlSa3d.Desktop
dotnet run
