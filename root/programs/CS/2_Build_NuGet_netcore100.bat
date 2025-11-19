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
call dotnet restore "Frameworks\Infrastructure\Nuget_netcore100.sln"
call dotnet msbuild %COMMANDLINE% "Frameworks\Infrastructure\Nuget_netcore100.sln"

call dotnet restore "Frameworks\Infrastructure\Nuget_RichClient_netcore100.sln"
call dotnet msbuild %COMMANDLINE% "Frameworks\Infrastructure\Nuget_RichClient_netcore100.sln"

pause

rem -------------------------------------------------------
endlocal