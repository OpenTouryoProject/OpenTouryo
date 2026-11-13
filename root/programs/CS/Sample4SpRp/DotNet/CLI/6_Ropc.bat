@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in ..\..\README.md (section 4).
rem
rem Resource Owner Password Credentials (RFC 6749 4.3) with SpRp_CLI (#668).
rem NOT RECOMMENDED (RFC 9700 2.4). Ported from the MPAS self-test; use device or pkce instead.
rem The password is read from the console (not echoed).
rem The IdP stub must be started with ..\..\container\Stub\Sample4SpRp.stub.json (it enables ROPC).
rem The user name is not the e-mail address (the test user is "tanaka").
rem Usage: 6_Ropc.bat [user] [net48|net10.0]   (default: tanaka, net10.0)

setlocal
set ROPCUSER=%~1
if "%ROPCUSER%"=="" set ROPCUSER=tanaka
set TFM=%~2
if "%TFM%"=="" set TFM=net10.0
call "%~dp0z_Run.bat" ropc %TFM% %ROPCUSER%
set RC=%ERRORLEVEL%
pause
endlocal & exit /b %RC%
