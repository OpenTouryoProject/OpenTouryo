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
rem Batch build of WSServer_sample.
rem --------------------------------------------------
dotnet restore "Samples4NetCore\Legacy\WSServer_sample\WSServer_sample.sln"
dotnet msbuild %COMMANDLINE% "Samples4NetCore\Legacy\WSServer_sample\WSServer_sample.sln"

pause

rem -------------------------------------------------------
endlocal
