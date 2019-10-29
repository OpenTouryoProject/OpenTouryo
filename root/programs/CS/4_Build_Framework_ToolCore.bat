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

rem -------------------------------------------------------
endlocal
rem --------------------------------------------------
rem Batch build of DPQuery_Tool.
rem --------------------------------------------------
call dotnet restore "Frameworks\Tools\DPQuery_Tool\DPQuery_ToolCore.sln"
call dotnet msbuild %COMMANDLINE% "Frameworks\Tools\DPQuery_Tool\DPQuery_ToolCore.sln"

pause

rem --------------------------------------------------
rem Batch build of DaoGen_Tool.
rem --------------------------------------------------
call dotnet restore "Frameworks\Tools\DaoGen_Tool\DaoGen_ToolCore.sln"
call dotnet msbuild %COMMANDLINE% "Frameworks\Tools\DaoGen_Tool\DaoGen_ToolCore.sln"

pause

rem --------------------------------------------------
rem Batch build of Deploy ZipPack With HTTP.
rem --------------------------------------------------
call dotnet restore "Frameworks\Tools\DeployZipPackWithHTTP\DeployZipPackWithHTTP_Core.sln"
call dotnet msbuild %COMMANDLINE% "Frameworks\Tools\DeployZipPackWithHTTP\DeployZipPackWithHTTP_Core.sln"

pause

rem --------------------------------------------------
rem Batch build of Workflow.
rem --------------------------------------------------
call dotnet restore "Frameworks\Tools\Workflow_Tool\Workflow_ToolCore.sln"
call dotnet msbuild %COMMANDLINE% "Frameworks\Tools\Workflow_Tool\Workflow_ToolCore.sln"

pause

rem -------------------------------------------------------
endlocal
