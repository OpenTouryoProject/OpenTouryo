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
rem Output xcopy after you build the batch Infrastructure
rem --------------------------------------------------
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Infrastructure\BusinessRichClient_net46.sln"

pause

rem -------------------------------------------------------
endlocal
