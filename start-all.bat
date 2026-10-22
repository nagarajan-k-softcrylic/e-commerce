@echo off
title Launching E-Commerce Microservices & UI
echo ========================================================
echo  Launching E-Commerce Microservices & Blazor WebApp
echo ========================================================
echo.
powershell -ExecutionPolicy Bypass -File "%~dp0scripts\run-services.ps1"
