@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in ..\README.md.
rem
rem Starts SpRp_React (Vite dev server) on http://localhost:5173 (#649).
rem The port must be 5173: it is the redirect_uri_code of SPA_Application in the MPAS template.
rem --strictPort makes Vite fail instead of moving to 5174 (which would not match the redirect URI).
rem Usage: StartUp.bat   (stop with Ctrl+C)

setlocal
pushd "%~dp0"

netstat -ano | findstr /r /c:":5173 .*LISTENING" >nul
if not errorlevel 1 (
    echo [ERROR] Port 5173 is already in use. Stop the other app ^(SpRp_Blazor?^) first.
    set RC=1
    goto :end
)

if not exist node_modules (
    echo === npm ci ^(first time only. installs exactly what package-lock.json says^) ===
    call npm ci
    if errorlevel 1 (
        echo [ERROR] npm ci failed.
        set RC=1
        goto :end
    )
)

echo === SpRp_React : http://localhost:5173 ===
echo   IdP stub must be running: https://localhost:44310/.well-known/openid-configuration
echo.
call npm run dev -- --port 5173 --strictPort
set RC=%ERRORLEVEL%

:end
popd
pause
endlocal & exit /b %RC%