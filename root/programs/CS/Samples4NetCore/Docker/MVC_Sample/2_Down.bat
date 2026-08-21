@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem       Japanese explanations are in README.md.
rem
rem Stops and removes the container.
rem Named volumes (logs / data protection keys) are kept on purpose:
rem removing the keys invalidates auth cookies and sessions.
rem Use "docker compose down -v" if you really want to drop them.

setlocal
pushd "%~dp0"

docker compose down
set RC=%ERRORLEVEL%

popd
endlocal & exit /b %RC%
