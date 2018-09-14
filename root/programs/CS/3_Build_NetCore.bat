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
rem Build the batch Infrastructure
rem --------------------------------------------------
dotnet restore "Frameworks\Infrastructure\AllComponent_netcore.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Infrastructure\AllComponent_netcore.sln"

pause

rem -------------------------------------------------------
endlocal