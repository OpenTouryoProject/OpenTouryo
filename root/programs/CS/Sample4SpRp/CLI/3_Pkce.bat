@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in ..\README.md (section 4).
rem
rem Authorization Code + PKCE (RFC 7636) with a loopback redirect (RFC 8252) (#649).
rem The default browser opens; sign in and approve. The response comes back to
rem http://localhost:54649/ (must match redirect_uri_code in ..\Stub\sprp-stub.json).
rem Usage: 3_Pkce.bat [net48|net10.0]   (default: net10.0)

setlocal
call "%~dp0z_Run.bat" pkce %1
set RC=%ERRORLEVEL%
pause
endlocal & exit /b %RC%
