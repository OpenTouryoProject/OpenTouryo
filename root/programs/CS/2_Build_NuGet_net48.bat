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
rem Build the batch Infrastructure(Nuget48)
rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Infrastructure\Nuget_net48.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\Nuget_net48.sln"

..\nuget.exe restore "Frameworks\Infrastructure\Nuget_RichClient_net48.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\Nuget_RichClient_net48.sln"

pause

rem -------------------------------------------------------
endlocal
