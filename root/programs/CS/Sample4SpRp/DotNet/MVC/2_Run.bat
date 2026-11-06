@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in ..\..\README.md (section 4.2).
rem
rem Runs SpRp_MVC on https://localhost:44336 (#668). Open it in a browser and pick a flow.
rem The URL comes from appsettings.json (Kestrel section), and must match the redirect_uri
rem registered in ..\..\Stub\SpRp_MVC.stub.json.
rem Usage: 2_Run.bat   (Ctrl+C to stop)

setlocal
set OUT=%~dp0SpRp_MVC\bin\Debug\net10.0

if not exist "%OUT%\SpRp_MVC.dll" (
    echo [ERROR] Not built: %OUT%\SpRp_MVC.dll
    echo         Run 1_Build.bat first.
    set RC=1
    goto :end
)

rem Use the output folder as the content root (appsettings.json is copied there).
pushd "%OUT%"
echo Open https://localhost:44336/ in a browser.
dotnet SpRp_MVC.dll
set RC=%ERRORLEVEL%
popd

:end
pause
endlocal & exit /b %RC%
