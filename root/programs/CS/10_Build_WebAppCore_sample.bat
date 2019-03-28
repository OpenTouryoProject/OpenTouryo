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
rem Batch build of MVC_Sample.
rem --------------------------------------------------

set CURRENTDIR=%cd%
cd "Samples4NetCore\Backend\MVC_Sample\MVC_Sample"
if exist "node_modules" rd /s /q "node_modules"
call RestoreLib1.bat
call RestoreLib2.bat
cd %CURRENTDIR%

dotnet restore "Samples4NetCore\Backend\MVC_Sample\MVC_Sample.sln"
dotnet msbuild %COMMANDLINE% "Samples4NetCore\Backend\MVC_Sample\MVC_Sample.sln"

pause

rem --------------------------------------------------
rem Batch build of ASPNETWebService.
rem --------------------------------------------------

dotnet restore "Samples4NetCore\Backend\ASPNETWebService\ASPNETWebService.sln"
dotnet msbuild %COMMANDLINE% "Samples4NetCore\Backend\ASPNETWebService\ASPNETWebService.sln"

pause

rem -------------------------------------------------------
endlocal
