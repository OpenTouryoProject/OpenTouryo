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
rem Batch build of DPQuery_Tool.
rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Tools\DPQuery_Tool\DPQuery_Tool.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tools\DPQuery_Tool\DPQuery_Tool.sln"

pause

rem --------------------------------------------------
rem Batch build of DaoGen_Tool.
rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Tools\DaoGen_Tool\DaoGen_Tool.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tools\DaoGen_Tool\DaoGen_Tool.sln"

pause

rem --------------------------------------------------
rem Batch build of Deploy ZipPack With HTTP.
rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Tools\DeployZipPackWithHTTP\DeployZipPackWithHTTP.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tools\DeployZipPackWithHTTP\DeployZipPackWithHTTP.sln"

pause

rem -------------------------------------------------------
endlocal
