setlocal

@rem --------------------------------------------------
@rem Turn off the echo function.
@rem --------------------------------------------------
@echo off

@rem --------------------------------------------------
@rem Get the path to the executable file.
@rem --------------------------------------------------
set CURRENT_DIR="%~dp0"

@rem --------------------------------------------------
@rem Execution of the common processing.
@rem --------------------------------------------------
call %CURRENT_DIR%z_Common.bat

rem --------------------------------------------------
rem Change the packages.config.
rem --------------------------------------------------
call %CURRENT_DIR%z_ChangePackages_net45.bat

rem --------------------------------------------------
rem Get libraries from C# folder.
rem --------------------------------------------------
xcopy /E /Y "..\CSFrameworks\Infrastructure\Build_net45" "Frameworks\Infrastructure\Build_net45\"
del "Frameworks\Infrastructure\Build_net45\" /f /s /a- *Business*

rem --------------------------------------------------
rem Build the Infrastructures
rem --------------------------------------------------

..\nuget.exe restore "Frameworks\Infrastructure\Business_net45.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\Business_net45.sln"

pause

rem -------------------------------------------------------
endlocal
