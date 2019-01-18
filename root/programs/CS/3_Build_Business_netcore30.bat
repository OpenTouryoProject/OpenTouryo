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
call dotnet restore "Frameworks\Infrastructure\Business_netcore30.sln"
call dotnet msbuild %COMMANDLINE% "Frameworks\Infrastructure\Business_netcore30.sln"

pause

rem -------------------------------------------------------
endlocal