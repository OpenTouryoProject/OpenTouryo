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
rem Batch build of 2CSClientWin_sample.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Samples\2CS_sample\2CSClientWin_sample\2CSClientWin_sample.sln"

pause

rem --------------------------------------------------
rem Batch build of DenDaoAndBatUpd_sample.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Samples\2CS_sample\GenDaoAndBatUpd_sample\GenDaoAndBatUpd_sample.sln"

pause

rem --------------------------------------------------
rem Batch build of TimeStamp_sample.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Samples\2CS_sample\TimeStamp_sample\TimeStamp_sample.sln"

pause

rem --------------------------------------------------
rem Batch build of 2CSClientWPF_sample.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Samples\\2CS_sample\2CSClientWPF_sample\2CSClientWPF_sample.sln"

pause

rem --------------------------------------------------
rem Batch build of AsyncEvent_sample.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Samples\\2CS_sample\AsyncEvent_sample\AsyncEvent_sample.sln"

pause

rem --------------------------------------------------
rem Batch build of CustCtrl_sample.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Samples\\2CS_sample\CustCtrl_sample\CustCtrl_sample.sln"

pause

rem -------------------------------------------------------
endlocal
