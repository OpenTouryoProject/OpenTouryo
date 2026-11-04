@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in ..\README.md (section 4).
rem
rem Device Authorization Grant (RFC 8628) with SpRp_CLI (#649).
rem Shows a URL and a user code; open the URL in a browser, enter the code and approve.
rem Usage: 2_Device.bat [net48|net10.0]   (default: net10.0)

setlocal
call "%~dp0z_Run.bat" device %1
set RC=%ERRORLEVEL%
pause
endlocal & exit /b %RC%
