@echo off
rem NOTE: keep this file pure ASCII (root/programs/CODING.md section 4).
rem
rem Folders and files under node_modules are skipped (#669):
rem   npm packages keep their executables in "bin" (vite, typescript, ...), and deleting them
rem   leaves node_modules in place but broken, so npm ci is not run again.
rem   findstr needs "\\" for a literal backslash; a trailing "\" is added to the path so that
rem   the node_modules folder itself also matches.
rem   "dir /a-d" also lists hidden files, as "del /s" did ("for /R" would not).

SET EXTENTION=*.suo *.user *.tmp *.log *.bak *.skrold

@echo --------------------------------------------------
@echo Delete the %EXTENTION%.
@echo --------------------------------------------------

for /F "delims=" %%f in ('dir /s /b /a-d %EXTENTION% 2^>nul') do (
  echo "%%~f" | findstr /i /l /c:"\\node_modules\\" >nul || del /f /a- "%%~f"
)

@echo --------------------------------------------------
@echo Deleted the %EXTENTION%.
@echo --------------------------------------------------

pause
