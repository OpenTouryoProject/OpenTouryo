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
rem Build the batch Infrastructure
rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx.sln"

..\nuget.exe restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx45.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx45.sln"

..\nuget.exe restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx47.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx47.sln"

dotnet restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUICore.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUICore.sln"

pause

rem -------------------------------------------------------
endlocal