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
rem Batch build of ServiceInterface(ASPNETWebServiceCore).
rem --------------------------------------------------
dotnet restore "Frameworks\Infrastructure\ServiceInterface\ASPNETWebServiceCore\ASPNETWebServiceCore.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Infrastructure\ServiceInterface\ASPNETWebServiceCore\ASPNETWebServiceCore.sln"

pause

rem -------------------------------------------------------
endlocal
