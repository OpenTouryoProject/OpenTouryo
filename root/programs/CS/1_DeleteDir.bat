@echo off

@echo --------------------------------------------------
@echo Delete the packages, obj, bin, bld, Temp, Build, PrecompiledWeb, .vs folders.
@echo --------------------------------------------------

for /D /R %%i in ( packages ) do (
  if exist "%%~i" RD /S /Q "%%~i"
)
for /D /R %%i in ( obj ) do (
  if exist "%%~i" RD /S /Q "%%~i"
)
for /D /R %%i in ( bin ) do (
  if exist "%%~i" RD /S /Q "%%~i"
)
for /D /R %%i in ( bld ) do (
  if exist "%%~i" RD /S /Q "%%~i"
)
for /D /R %%i in ( Temp ) do (
  if exist "%%~i" RD /S /Q "%%~i"
)
for /D /R %%i in ( Build ) do (
  if exist "%%~i" RD /S /Q "%%~i"
)
for /D /R %%i in ( PrecompiledWeb ) do (
  if exist "%%~i" RD /S /Q "%%~i"
)
for /D /R %%i in ( .vs ) do (
  if exist "%%~i" RD /S /Q "%%~i"
)
pause
