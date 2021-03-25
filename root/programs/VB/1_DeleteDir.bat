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

set DIRECTORIES=Build, Build_net45, Build_net46, Build_net47, Build_net48, Build_netstd20, Build_netstd21, Build_netcore20, Build_netcore30, Build_netcore50
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