@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in ..\..\README.md (section 4.2).
rem
rem Builds SpRp_MVC for net10.0 (#668).
rem Usage: 1_Build.bat

setlocal
pushd "%~dp0"

set FX_DIR=..\..\..\Frameworks\Infrastructure

echo === 1/2 : checking prerequisites ===

if not exist "%FX_DIR%\Build_netcore100\net10.0\OpenTouryo.Framework.dll" (
    echo [ERROR] Framework ^(net10.0^) is not built: %FX_DIR%\Build_netcore100\net10.0
    echo         Build the framework first ^(root\programs\1_BuildAll.ps1^).
    goto :fail
)

echo === 2/2 : dotnet build ===
dotnet build SpRp_MVC.sln -nologo
if errorlevel 1 goto :fail

echo.
echo [OK] Built: SpRp_MVC\bin\Debug\net10.0\SpRp_MVC.dll
echo      Next: start the IdP stub with the SpRp_MVC clients
echo            ^(..\..\Stub\StubCompose.ps1 -Settings ..\..\Stub\Sample4SpRp.stub.json^), then 2_Run.bat.
set RC=0
goto :end

:fail
echo.
echo [NG] Build failed.
set RC=1

:end
popd
pause
endlocal & exit /b %RC%
