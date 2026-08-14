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

@rem --------------------------------------------------
@rem Batch build of TestTransmission.
@rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Tests\TestTransmission\TestTransmissionFx48.sln" %NUGET_MSBUILD%
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\TestTransmission\TestTransmissionFx48.sln"

pause

rem -------------------------------------------------------
endlocal
