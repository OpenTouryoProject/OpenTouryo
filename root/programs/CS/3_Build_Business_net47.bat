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
call %CURRENT_DIR%z_ChangePackages_net47.bat

rem --------------------------------------------------
rem Build the Infrastructures
rem --------------------------------------------------

..\nuget.exe restore "Frameworks\Infrastructure\Business_net47.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\Business_net47.sln"

pause

rem -------------------------------------------------------
endlocal
