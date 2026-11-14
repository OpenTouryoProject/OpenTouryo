@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in ..\..\README.md (section 4).
rem
rem Client Credentials (RFC 6749 4.4) with SpRp_CLI (#668).
rem A confidential client that models a batch on a server (secret in the config file).
rem The IdP stub must be started with ..\..\container\Stub\Sample4SpRp.stub.json.
rem Usage: 4_ClientCredentials.bat [net48|net10.0]   (default: net10.0)

setlocal
call "%~dp0z_Run.bat" cc %1
set RC=%ERRORLEVEL%
pause
endlocal & exit /b %RC%
