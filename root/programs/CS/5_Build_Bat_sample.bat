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
rem Batch build of SimpleBatch_sample.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Samples\Bat_sample\SimpleBatch_sample\SimpleBatch_sample.sln"

pause

rem --------------------------------------------------
rem Batch build of RerunnableBatch_sample.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Samples\Bat_sample\RerunnableBatch_sample\RerunnableBatch_sample.sln"

pause

rem --------------------------------------------------
rem Batch build of RerunnableBatch_sample2.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Samples\Bat_sample\RerunnableBatch_sample2\RerunnableBatch_sample2.sln"

pause

rem --------------------------------------------------
rem Batch build of RerunnableBatch_sample3.
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Samples\Bat_sample\RerunnableBatch_sample3\RerunnableBatch_sample3.sln"

pause

rem -------------------------------------------------------
endlocal
