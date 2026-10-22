<#
.SYNOPSIS
    Automated Azure SQL Server & Databases Provisioning Script for E-Commerce Microservices.
.DESCRIPTION
    Creates:
      1. Resource Group
      2. Azure SQL Logical Server
      3. Firewall Rules (Allow Azure Services + Local Client IP)
      4. Ecommerce_OrderDb (Serverless / Basic)
      5. Ecommerce_InventoryDb (Serverless / Basic)
#>

param(
    [string]$ResourceGroupName = "rg-ecommerce-microservices",
    [string]$Location = "eastus",
    [string]$SqlServerName = "sql-ecommerce-$((Get-Random -Minimum 1000 -Maximum 9999))",
    [string]$AdminUser = "sqladmin",
    [string]$AdminPassword = "P@ssw0rd$((Get-Random -Minimum 1000 -Maximum 9999))!"
)

Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host " Provisioning Azure SQL for E-Commerce Microservices   " -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "Resource Group: $ResourceGroupName" -ForegroundColor Yellow
Write-Host "Location:       $Location" -ForegroundColor Yellow
Write-Host "SQL Server:     $SqlServerName.database.windows.net" -ForegroundColor Yellow
Write-Host "Admin User:     $AdminUser" -ForegroundColor Yellow
Write-Host "Admin Password: $AdminPassword" -ForegroundColor Yellow
Write-Host "=======================================================" -ForegroundColor Cyan

# 1. Login check
Write-Host "`n1. Checking Azure Authentication..." -ForegroundColor Green
$account = az account show --output json 2>$null | ConvertFrom-Json
if (-not $account) {
    Write-Host "Not logged in. Initiating Azure login..." -ForegroundColor Yellow
    az login --output table
}

# 2. Create Resource Group
Write-Host "`n2. Creating Resource Group: $ResourceGroupName..." -ForegroundColor Green
az group create --name $ResourceGroupName --location $Location --output table

# 3. Create Azure SQL Logical Server
Write-Host "`n3. Creating Azure SQL Server: $SqlServerName..." -ForegroundColor Green
az sql server create `
    --name $SqlServerName `
    --resource-group $ResourceGroupName `
    --location $Location `
    --admin-user $AdminUser `
    --admin-password $AdminPassword `
    --output table

# 4. Configure Firewall Rules
Write-Host "`n4. Configuring Firewall Rules..." -ForegroundColor Green
# Allow Azure internal services
az sql server firewall-rule create `
    --resource-group $ResourceGroupName `
    --server $SqlServerName `
    --name "AllowAzureServices" `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0 `
    --output table

# Allow current client IP address
$clientIp = (Invoke-RestMethod -Uri "https://api.ipify.org" -UseBasicParsing).Trim()
Write-Host "Detected your public IP: $clientIp" -ForegroundColor Yellow
az sql server firewall-rule create `
    --resource-group $ResourceGroupName `
    --server $SqlServerName `
    --name "ClientIPRule" `
    --start-ip-address $clientIp `
    --end-ip-address $clientIp `
    --output table

# 5. Create Ecommerce_OrderDb
Write-Host "`n5. Creating Database: Ecommerce_OrderDb..." -ForegroundColor Green
az sql db create `
    --resource-group $ResourceGroupName `
    --server $SqlServerName `
    --name "Ecommerce_OrderDb" `
    --service-objective "Basic" `
    --output table

# 6. Create Ecommerce_InventoryDb
Write-Host "`n6. Creating Database: Ecommerce_InventoryDb..." -ForegroundColor Green
az sql db create `
    --resource-group $ResourceGroupName `
    --server $SqlServerName `
    --name "Ecommerce_InventoryDb" `
    --service-objective "Basic" `
    --output table

# 7. Print Connection Strings
$orderConn = "Server=tcp:$SqlServerName.database.windows.net,1433;Initial Catalog=Ecommerce_OrderDb;Persist Security Info=False;User ID=$AdminUser;Password=$AdminPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
$inventoryConn = "Server=tcp:$SqlServerName.database.windows.net,1433;Initial Catalog=Ecommerce_InventoryDb;Persist Security Info=False;User ID=$AdminUser;Password=$AdminPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

Write-Host "`n=======================================================" -ForegroundColor Cyan
Write-Host " PROVISIONING COMPLETE! YOUR AZURE CONNECTION STRINGS: " -ForegroundColor Cyan
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host "`n[Order Service Connection String]:" -ForegroundColor Green
Write-Host $orderConn -ForegroundColor White
Write-Host "`n[Inventory Service Connection String]:" -ForegroundColor Green
Write-Host $inventoryConn -ForegroundColor White
Write-Host "`nUpdate 'src\Ecommerce.OrderService\Ecommerce.OrderService.Api\appsettings.json' and 'src\Ecommerce.InventoryService\Ecommerce.InventoryService.Api\appsettings.json' with these strings and set 'UseInMemory': false." -ForegroundColor Yellow

