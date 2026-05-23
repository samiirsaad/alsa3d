#!/bin/bash

echo "=========================================="
echo "  Al-Sa3d Accounting System - Launcher"
echo "=========================================="
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}[1/4] Restoring NuGet Packages...${NC}"
dotnet restore
if [ $? -ne 0 ]; then
    echo -e "${RED}ERROR: Failed to restore packages.${NC}"
    exit 1
fi

echo -e "${GREEN}[2/4] Building the solution...${NC}"
dotnet build --no-restore
if [ $? -ne 0 ]; then
    echo -e "${RED}ERROR: Build failed.${NC}"
    exit 1
fi

echo -e "${YELLOW}[3/4] Database Setup (Optional - Requires SQL Server)${NC}"
if command -v sqlcmd &> /dev/null; then
    echo "sqlcmd found. Attempting database setup..."
    DB_SERVER="localhost"
    
    echo "Creating Database..."
    sqlcmd -S $DB_SERVER -E -i database/01_Create_Database.sql
    
    echo "Creating Tables..."
    sqlcmd -S $DB_SERVER -E -i database/02_Create_Tables.sql
    
    echo "Seeding Data..."
    sqlcmd -S $DB_SERVER -E -i database/03_Seed_Data.sql
    
    echo "Creating Stored Procedures..."
    sqlcmd -S $DB_SERVER -E -i database/04_Stored_Procedures.sql
    
    echo "Creating Views & Functions..."
    sqlcmd -S $DB_SERVER -E -i database/05_Views_Functions.sql
    
    echo -e "${GREEN}Database setup completed.${NC}"
else
    echo -e "${YELLOW}WARNING: sqlcmd not found. Skipping database setup.${NC}"
    echo "Please manually run the SQL scripts in the 'database' folder on your SQL Server."
fi

echo -e "${GREEN}[4/4] Launching Al-Sa3d Application...${NC}"
cd src/AlSa3d.Desktop
dotnet run
