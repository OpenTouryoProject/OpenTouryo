@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in README.md.
rem
rem Host-build type: publish on the host, then copy into the runtime image.
rem See README.md and #548.

setlocal
pushd "%~dp0"

set SLN_DIR=..\..\Backend\MVC_Sample\MVC_Sample
set FX_DLL=..\..\..\Frameworks\Infrastructure\Build_netcore100\net10.0\OpenTouryo.Framework.dll

echo === 1/4 : checking prerequisites ===

if not exist "%FX_DLL%" (
    echo [ERROR] Framework is not built: %FX_DLL%
    echo         Build the framework first ^(root\programs\1_BuildAll.ps1^).
    goto :fail
)

if not exist ".\https\aspnetapp.pem" (
    echo [ERROR] .\https\aspnetapp.pem not found.
    echo         Run 0_SetupCert.ps1 first.
    goto :fail
)

docker network inspect common_link >nul 2>&1
if errorlevel 1 (
    echo [ERROR] docker network "common_link" not found.
    echo         Start LocalServicesOnDocker first ^(it owns the network and the DB^).
    echo         https://github.com/NetDevInfraWGinOSSConsortium/LocalServicesOnDocker
    goto :fail
)

echo === 2/4 : dotnet publish ===
if exist ".\publish" rmdir /s /q ".\publish"
dotnet publish "%SLN_DIR%\MVC_Sample.csproj" -c Release -o ".\publish"
if errorlevel 1 goto :fail

if not exist ".\publish\MVC_Sample.dll" (
    echo [ERROR] publish output not found: .\publish\MVC_Sample.dll
    goto :fail
)

echo === 3/4 : docker compose up --build ===
docker compose up --build -d
if errorlevel 1 goto :fail

echo === 4/4 : done ===
echo   HTTP  : http://localhost:8080  ^(redirects to HTTPS^)
echo   HTTPS : https://localhost:8081
echo.
echo   logs  : docker compose logs -f
echo   stop  : 2_Down.bat
popd
endlocal
exit /b 0

:fail
echo.
echo [FAILED]
popd
endlocal
exit /b 1
