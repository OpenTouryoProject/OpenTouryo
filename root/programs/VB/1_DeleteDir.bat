@echo off

set DIRECTORIES=packages, obj, bin, bld, Temp, Build, PrecompiledWeb, .vs
@echo --------------------------------------------------
@echo Delete the %DIRECTORIES% folders.
@echo --------------------------------------------------

@rem カンマをスペースに変換
set w1=%DIRECTORIES:,= %

@rem 連続したスペースを、スペース１個に変換
set w2=%w1:  = %

for %%a in ( %w2% ) do (
  for /D /R %%i in ( %%a ) do (
    if exist "%%~i" RD /S /Q "%%~i"
  )
)

pause

set DIRECTORIES=Build
@echo --------------------------------------------------
@echo Delete the %DIRECTORIES% folders.
@echo --------------------------------------------------

@rem カンマをスペースに変換
set w1=%DIRECTORIES:,= %

@rem 連続したスペースを、スペース１個に変換
set w2=%w1:  = %

for %%a in ( %w2% ) do (
  for /D /R %%i in ( %%a ) do (
    if exist "%%~i" RD /S /Q "%%~i"
  )
)

pause