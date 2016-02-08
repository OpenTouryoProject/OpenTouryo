setlocal

@rem --------------------------------------------------
@rem Turn off the echo function.
@rem --------------------------------------------------
@echo off

@rem --------------------------------------------------
@rem Save the value of the PATH environment variable.
@rem --------------------------------------------------
set ORG_PATH=%PATH% 

@rem --------------------------------------------------
@rem Get the path to the executable file.
@rem --------------------------------------------------
set CURRENT_DIR="%~dp0"

@rem --------------------------------------------------
@rem Execution of the common processing.
@rem --------------------------------------------------
call %CURRENT_DIR%z_Common.bat

rem --------------------------------------------------
rem Copy the Bin folder assembly.
rem --------------------------------------------------

md "Frameworks\Infrastructure\ServiceInterface\ASPNETWebService\Bin"
xcopy /E /Y "Samples\WS_sample\Build" "Frameworks\Infrastructure\ServiceInterface\ASPNETWebService\Bin\"
pause

rem --------------------------------------------------
rem Batch build of ServiceInterface(ASPNETWebService).
rem --------------------------------------------------
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
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\ServiceInterface\WCFService\WCFService.sln"

pause

rem -------------------------------------------------------
endlocal
