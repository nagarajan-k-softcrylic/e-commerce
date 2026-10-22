@echo off
title Stopping E-Commerce Services
echo Stopping all running instances on ports 5000, 5101, 5102, 5200...
powershell -Command "Get-Process -Name dotnet -ErrorAction SilentlyContinue | Where-Object { $_.CommandLine -like '*Ecommerce*' } | Stop-Process -Force"
echo Services stopped successfully.
pause

