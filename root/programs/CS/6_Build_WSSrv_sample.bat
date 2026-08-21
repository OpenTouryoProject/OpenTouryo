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
md "Samples\WS_sample\Temp"
md "Samples\WS_sample\Build"

rem --------------------------------------------------
rem Batch build of WSServer_sample.
rem --------------------------------------------------
..\nuget.exe restore "Samples\WS_sample\WSServer_sample\WSServer_sample.sln" %NUGET_MSBUILD%
%BUILDFILEPATH% %COMMANDLINE% "Samples\WS_sample\WSServer_sample\WSServer_sample.sln"

xcopy /E /Y "Samples\WS_sample\WSServer_sample\bin\%BUILD_CONFIG%" "Samples\WS_sample\Temp\%BUILD_CONFIG%\"
xcopy /E /Y "Samples\WS_sample\Temp\%BUILD_CONFIG%" "Samples\WS_sample\Build\"

@rem --------------------------------------------------
@rem Batch build of ASPNETWebService(ResourceServer).
@rem --------------------------------------------------
@rem Not copied to Build\ : nothing else references it, so just build it.
..\nuget.exe restore "Samples\WS_sample\ASPNETWebService\ASPNETWebService.sln" %NUGET_MSBUILD%
%BUILDFILEPATH% %COMMANDLINE% "Samples\WS_sample\ASPNETWebService\ASPNETWebService.sln"

pause

rem -------------------------------------------------------
endlocal
