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
call %CURRENT_DIR%z_Common2.bat

rem --------------------------------------------------
rem Batch build of UWP_sample.
rem --------------------------------------------------
..\nuget.exe restore "Samples\UWP_sample\UWP_sample.sln"
%BUILDFILEPATH% %COMMANDLINE% "Samples\UWP_sample\UWP_sample.sln"

pause

rem -------------------------------------------------------
endlocal
