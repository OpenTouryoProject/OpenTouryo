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
@rem Batch build of EncAndDecUtilCUI.
@rem --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI"
call copy_cert.bat
cd %CURRENTDIR%

..\nuget.exe restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx48.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx48.sln"

dotnet restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUICore80.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUICore80.sln"

@echo --------------------------------------------------
@echo Test the EncAndDecUtilCUIFx(48).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\net48\bin\Debug"
"EncAndDecUtilCUIFx.exe" > ..\..\..\Result48.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the EncAndDecUtilCUICore(80).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\core80\bin\Debug\net8.0"
dotnet "EncAndDecUtilCUICore.dll" > ..\..\..\..\ResultCore80.txt
cd %CURRENTDIR%

pause

rem -------------------------------------------------------
endlocal