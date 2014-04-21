@echo off

@echo --------------------------------------------------
@echo Delete the obj, bin, Temp, Build, PrecompiledWeb folders.
@echo --------------------------------------------------

for /D /R %%i in ( obj ) do (
  if exist "%%~i" RD /S /Q "%%~i"
)
for /D /R %%i in ( bin ) do (
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

@echo --------------------------------------------------
@echo Deleted the obj, bin, Temp, Build, PrecompiledWeb folders.
@echo --------------------------------------------------

pause
