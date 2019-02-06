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
rem Batch build of EncAndDecUtil.
rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Tests\EncAndDecUtil\EncAndDecUtil45.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\EncAndDecUtil\EncAndDecUtil45.sln"
..\nuget.exe restore "Frameworks\Tests\EncAndDecUtil\EncAndDecUtil46.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\EncAndDecUtil\EncAndDecUtil46.sln"
..\nuget.exe restore "Frameworks\Tests\EncAndDecUtil\EncAndDecUtil47.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\EncAndDecUtil\EncAndDecUtil47.sln"

pause

rem -------------------------------------------------------
endlocal