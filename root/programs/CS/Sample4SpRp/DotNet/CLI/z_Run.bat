@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in ..\..\README.md (section 4).
rem
rem Common runner called by 2_Device.bat / 3_Pkce.bat (#649).
rem Usage: z_Run.bat <device|pkce> [net48|net10.0]   (default: net10.0)
rem Exit code: 0 = OK, 1 = flow failed, 2 = usage error (same as SpRp_CLI).

setlocal
pushd "%~dp0"

set FLOW=%~1
set TFM=%~2
if "%TFM%"=="" set TFM=net10.0

if /i not "%FLOW%"=="device" if /i not "%FLOW%"=="pkce" (
    echo [ERROR] Unknown flow: "%FLOW%" ^(device or pkce^)
    set RC=2
    goto :end
)

set OUT=SpRp_CLI\bin\Debug\%TFM%

if /i "%TFM%"=="net48" (
    set EXE=%OUT%\SpRp_CLI.exe
) else if /i "%TFM%"=="net10.0" (
    set EXE=%OUT%\SpRp_CLI.dll
) else (
    echo [ERROR] Unknown target framework: "%TFM%" ^(net48 or net10.0^)
    set RC=2
    goto :end
)

if not exist "%EXE%" (
    echo [ERROR] Not built: %EXE%
    echo         Run 1_Build.bat first.
    set RC=1
    goto :end
)

echo === SpRp_CLI %FLOW% ^(%TFM%^) ===
echo   IdP stub must be running: https://localhost:44310/.well-known/openid-configuration
echo.

if /i "%TFM%"=="net48" (
    "%EXE%" %FLOW%
) else (
    dotnet "%EXE%" %FLOW%
)
set RC=%ERRORLEVEL%

echo.
if "%RC%"=="0" (
    echo [OK] %FLOW% ^(%TFM%^)
) else (
    echo [NG] %FLOW% ^(%TFM%^) exit code %RC%
)

:end
popd
endlocal & exit /b %RC%
