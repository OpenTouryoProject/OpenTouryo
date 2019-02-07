@echo off

rem --------------------------------------------------
rem Get libraries from C# folder.
rem --------------------------------------------------
xcopy /E /Y "..\CS\Frameworks\Infrastructure\Build_net45" "Frameworks\Infrastructure\Build_net45\"
xcopy /E /Y "..\CS\Frameworks\Infrastructure\Build_net46" "Frameworks\Infrastructure\Build_net46\"
xcopy /E /Y "..\CS\Frameworks\Infrastructure\Build_net47" "Frameworks\Infrastructure\Build_net47\"
del /f /s /a- "Frameworks\Infrastructure\*.Business.*"
del /f /s /a- "Frameworks\Infrastructure\*.CustomControl.*"

pause
