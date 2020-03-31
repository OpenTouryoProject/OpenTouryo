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
rem Make the Directory.
rem --------------------------------------------------
md "Samples4NetCore\Legacy\WS_sample\Temp"
md "Samples4NetCore\Legacy\WS_sample\Build"

rem --------------------------------------------------
rem Batch build of WSServer_sample.
rem --------------------------------------------------
dotnet restore "Samples4NetCore\Legacy\WS_sample\WSServer_sample\WSServer_sample.sln"
dotnet msbuild %COMMANDLINE% "Samples4NetCore\Legacy\WS_sample\WSServer_sample\WSServer_sample.sln"

xcopy /E /Y "Samples4NetCore\Legacy\WS_sample\WSServer_sample\bin\%BUILD_CONFIG%\netcoreapp3.0" "Samples4NetCore\Legacy\WS_sample\Temp\%BUILD_CONFIG%\netcoreapp3.0\"
xcopy /E /Y "Samples4NetCore\Legacy\WS_sample\Temp\%BUILD_CONFIG%\netcoreapp3.0" "Samples4NetCore\Legacy\WS_sample\Build\netcoreapp3.0\"

pause

rem -------------------------------------------------------
endlocal
