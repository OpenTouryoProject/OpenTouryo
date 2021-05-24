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
rem Batch build of Simple_CLI.
rem --------------------------------------------------
dotnet restore "Samples4NetCore\Legacy\CLI_sample\Simple_CLI\Simple_CLI.sln"
dotnet msbuild %COMMANDLINE% "Samples4NetCore\Legacy\CLI_sample\Simple_CLI\Simple_CLI.sln"

pause

rem --------------------------------------------------
rem Batch build of DAG_Login_CLI.
rem --------------------------------------------------
dotnet restore "Samples4NetCore\Legacy\CLI_sample\DAG_Login_CLI\DAG_Login_CLI.sln"
dotnet msbuild %COMMANDLINE% "Samples4NetCore\Legacy\CLI_sample\DAG_Login_CLI\DAG_Login_CLI.sln"

pause

rem --------------------------------------------------
rem Batch build of LIR _Login_CLI.
rem --------------------------------------------------
dotnet restore "Samples4NetCore\Legacy\CLI_sample\LIR_Login_CLI\LIR_Login_CLI.sln"
dotnet msbuild %COMMANDLINE% "Samples4NetCore\Legacy\CLI_sample\LIR_Login_CLI\LIR_Login_CLI.sln"

pause

rem -------------------------------------------------------
endlocal
