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
set CURRENT_DIR=%~dp0

@rem --------------------------------------------------
@rem Execution of the common processing.
@rem --------------------------------------------------
call %CURRENT_DIR%z_Common.bat

rem --------------------------------------------------
rem Copy the Bin folder assembly.
rem --------------------------------------------------
md "Samples\WS_sample\WSClient_sample\WSClientSL_samples\WSClientSL_sample.Web\Bin"
xcopy /E /Y "Frameworks\Infrastructure\Build" "Samples\WS_sample\WSClient_sample\WSClientSL_samples\WSClientSL_sample.Web\Bin\"
pause

rem --------------------------------------------------
rem Batch build of WSClientSL_sample.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Samples\WS_sample\WSClient_sample\WSClientSL_samples\WSClientSL_samples.sln"

pause

rem -------------------------------------------------------
endlocal