@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in ..\..\README.md (section 4).
rem
rem JWT Bearer (RFC 7523 2.1) with SpRp_CLI (#668).
rem The assertion is signed with the client key (SpRp_RsaPfxFilePath).
rem The IdP stub must be started with ..\..\container\Stub\Sample4SpRp.stub.json.
rem Usage: 5_JwtBearer.bat [net48|net10.0]   (default: net10.0)

setlocal
call "%~dp0z_Run.bat" jwtbearer %1
set RC=%ERRORLEVEL%
pause
endlocal & exit /b %RC%
