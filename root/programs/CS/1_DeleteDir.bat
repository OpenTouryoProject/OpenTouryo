@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem
rem Folders and files under node_modules are skipped (#669):
rem   npm packages keep their executables in "bin" (vite, typescript, ...), and deleting them
rem   leaves node_modules in place but broken, so npm ci is not run again.
rem   findstr needs "\\" for a literal backslash; a trailing "\" is added to the path so that
rem   the node_modules folder itself also matches.

set DIRECTORIES=packages, obj, bin, bld, Temp, PrecompiledWeb, MigrationBackup, .vs
@echo --------------------------------------------------
@echo Delete the %DIRECTORIES% folders.
@echo --------------------------------------------------

@rem Turn the commas into spaces.
set w1=%DIRECTORIES:,= %

@rem Collapse runs of spaces into a single space.
set w2=%w1:  = %

for %%a in ( %w2% ) do (
  for /D /R %%i in ( %%a ) do (
    if exist "%%~i" (
      echo "%%~i\" | findstr /i /l /c:"\\node_modules\\" >nul || RD /S /Q "%%~i"
    )
  )
)

pause
