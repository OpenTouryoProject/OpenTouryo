@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in ..\README.md.
rem
rem Starts SpRp_Blazor (Blazor WebAssembly dev server) on http://localhost:5173 (#649).
rem The port must be 5173: it is the redirect_uri_code of SPA_Application in the MPAS template.
rem The URL is given here, because Properties\launchSettings.json is ignored by the repository's .gitignore
rem (it may not exist after a clone).
rem Usage: StartUp.bat   (stop with Ctrl+C)

setlocal
pushd "%~dp0"

netstat -ano | findstr /r /c:":5173 .*LISTENING" >nul
if not errorlevel 1 (
    echo [ERROR] Port 5173 is already in use. Stop the other app ^(SpRp_React?^) first.
    set RC=1
    goto :end
)

echo === SpRp_Blazor : http://localhost:5173 ===
echo   IdP stub must be running: https://localhost:44310/.well-known/openid-configuration
echo.
set ASPNETCORE_ENVIRONMENT=Development
dotnet run --no-launch-profile --urls http://localhost:5173
set RC=%ERRORLEVEL%

:end
popd
pause
endlocal & exit /b %RC%