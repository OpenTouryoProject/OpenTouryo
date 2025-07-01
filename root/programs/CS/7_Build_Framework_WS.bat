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
rem Batch build of ServiceInterface(ASPNETWebService).
rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Infrastructure\ServiceInterface\ASPNETWebService\ASPNETWebService.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\ServiceInterface\ASPNETWebService\ASPNETWebService.sln"

pause

rem --------------------------------------------------
rem Copy the dll folder assembly.
rem --------------------------------------------------

md "Frameworks\Infrastructure\ServiceInterface\WCFService\dll"
xcopy /E /Y "Samples\WS_sample\Build" "Frameworks\Infrastructure\ServiceInterface\WCFService\dll\"
pause

rem --------------------------------------------------
rem Batch build of ServiceInterface(WCFService).
rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Infrastructure\ServiceInterface\WCFService\WCFService.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\ServiceInterface\WCFService\WCFService.sln"

pause

rem -------------------------------------------------------
endlocal
