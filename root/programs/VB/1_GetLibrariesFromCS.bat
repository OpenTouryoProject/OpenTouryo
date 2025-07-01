@echo off

rem --------------------------------------------------
rem Get libraries from C# folder.
rem --------------------------------------------------
xcopy /E /Y "..\CS\Frameworks\Infrastructure\Build_net48" "Frameworks\Infrastructure\Build_net48\"
del /f /s /a- "Frameworks\Infrastructure\*.Business.*"
del /f /s /a- "Frameworks\Infrastructure\*.CustomControl.*"

pause
