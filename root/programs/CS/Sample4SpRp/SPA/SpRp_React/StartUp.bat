@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in ..\README.md.
rem
rem Starts SpRp_React (Vite dev server) on http://localhost:5173 (#649).
rem The port must be 5173: it is the redirect_uri_code of SPA_Application in the MPAS template.
rem --strictPort makes Vite fail instead of moving to 5174 (which would not match the redirect URI).
rem node_modules is (re)installed with npm ci when it is missing or broken (see below).
rem Usage: StartUp.bat   (stop with Ctrl+C)

setlocal
pushd "%~dp0"

netstat -ano | findstr /r /c:":5173 .*LISTENING" >nul
if not errorlevel 1 (
    echo [ERROR] Port 5173 is already in use. Stop the other app ^(SpRp_Blazor?^) first.
    set RC=1
    goto :end
)

rem Run npm ci when node_modules is missing, or when it is broken
rem (an interrupted npm ci, files deleted by hand, ...). The clean step of the build
rem (CS\1_DeleteDir.bat) used to delete node_modules\vite\bin as well; fixed in #669 (#668).
rem npm ci deletes node_modules itself before installing, so a broken one is replaced.
set NEED_CI=0
if not exist node_modules set NEED_CI=1
if exist node_modules if not exist node_modules\vite\bin\vite.js (
    echo [WARN] node_modules is broken ^(node_modules\vite\bin\vite.js is missing^). Reinstalling.
    set NEED_CI=1
)

if "%NEED_CI%"=="1" (
    echo === npm ci ^(installs exactly what package-lock.json says^) ===
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