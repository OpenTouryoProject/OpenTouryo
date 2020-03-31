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
rem Batch build of WSClientWin_sample.
rem --------------------------------------------------
dotnet restore "Samples4NetCore\Legacy\WS_sample\WSClient_sample\WSClientWin_sample\WSClientWin_sample.sln"
dotnet msbuild %COMMANDLINE% "Samples4NetCore\Legacy\WS_sample\WSClient_sample\WSClientWin_sample\WSClientWin_sample.sln"

pause

rem --------------------------------------------------
rem Batch build of WSClientWPF_sample.
rem --------------------------------------------------
rem dotnet restore "Samples4NetCore\Legacy\WS_sample\WSClient_sample\WSClientWinWPF_sample\WSClientWinWPF_sample.sln"
rem dotnet msbuild "Samples4NetCore\Legacy\WS_sample\WSClient_sample\WSClientWinWPF_sample\WSClientWinWPF_sample.sln"

pause

rem --------------------------------------------------
rem Batch build of WSClientWin2_sample.
rem --------------------------------------------------
dotnet restore "Samples4NetCore\Legacy\WS_sample\WSClient_sample\WSClientWin2_sample\WSClientWin2_sample.sln"
dotnet msbuild %COMMANDLINE% "Samples4NetCore\Legacy\WS_sample\WSClient_sample\WSClientWin2_sample\WSClientWin2_sample.sln"

pause

rem -------------------------------------------------------
endlocal
