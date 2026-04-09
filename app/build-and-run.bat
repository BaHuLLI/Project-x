@echo off
setlocal
cd /d "%~dp0"

echo [Project-X Pro Dash] restore...
dotnet restore "ProjectXProDash.csproj"
if errorlevel 1 exit /b %errorlevel%

echo [Project-X Pro Dash] build...
dotnet build "ProjectXProDash.csproj" -c Debug
if errorlevel 1 exit /b %errorlevel%

echo [Project-X Pro Dash] launch...
start "" "%~dp0bin\Debug\net8.0-windows\ProjectXProDash.exe"
exit /b 0
