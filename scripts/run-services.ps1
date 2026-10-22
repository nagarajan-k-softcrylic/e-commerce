# =========================================================================
# Ecommerce Microservices Application - Startup Orchestration Script
# =========================================================================

Write-Host "=================================================" -ForegroundColor Cyan
Write-Host " Starting E-Commerce Microservices Architecture   " -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan

$RootPath = Resolve-Path "$PSScriptRoot\.."

Write-Host "1. Starting Ecommerce.OrderService (Port: 5101)..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$RootPath\src\Ecommerce.OrderService\Ecommerce.OrderService.Api'; dotnet run --urls 'http://localhost:5101'"

Write-Host "2. Starting Ecommerce.InventoryService (Port: 5102)..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$RootPath\src\Ecommerce.InventoryService\Ecommerce.InventoryService.Api'; dotnet run --urls 'http://localhost:5102'"

Start-Sleep -Seconds 3

Write-Host "3. Starting Ecommerce.ApiGateway (Port: 5000)..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$RootPath\src\Ecommerce.ApiGateway'; dotnet run --urls 'http://localhost:5000'"

Start-Sleep -Seconds 3

Write-Host "4. Starting Ecommerce.WebApp Blazor WASM (Port: 5200)..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$RootPath\src\Ecommerce.WebApp'; dotnet run --urls 'http://localhost:5200'"

Write-Host ""
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host " Services Initialized! Endpoints:                " -ForegroundColor Cyan
Write-Host "  - Blazor WebApp:    http://localhost:5200      " -ForegroundColor Yellow
Write-Host "  - API Gateway:      http://localhost:5000      " -ForegroundColor Yellow
Write-Host "  - Order Service:    http://localhost:5101/swagger" -ForegroundColor Yellow
Write-Host "  - Inventory Service:http://localhost:5102/swagger" -ForegroundColor Yellow
Write-Host "=================================================" -ForegroundColor Cyan

