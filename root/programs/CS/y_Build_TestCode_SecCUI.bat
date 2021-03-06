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

..\nuget.exe restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx45.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx45.sln"

..\nuget.exe restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx46.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx46.sln"

..\nuget.exe restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx47.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx47.sln"

..\nuget.exe restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx48.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUIFx48.sln"

dotnet restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUICore20.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUICore20.sln"

dotnet restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUICore30.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUICore30.sln"

@echo --------------------------------------------------
@echo Test the EncAndDecUtilCUIFx(45).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\net45\bin\Debug"
"EncAndDecUtilCUIFx.exe" > ..\..\..\Result45.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the EncAndDecUtilCUIFx(46).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\net46\bin\Debug"
"EncAndDecUtilCUIFx.exe" > ..\..\..\Result46.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the EncAndDecUtilCUIFx(47).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\net47\bin\Debug"
"EncAndDecUtilCUIFx.exe" > ..\..\..\Result47.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the EncAndDecUtilCUIFx(48).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\net48\bin\Debug"
"EncAndDecUtilCUIFx.exe" > ..\..\..\Result48.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the EncAndDecUtilCUICore(20).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\core20\bin\Debug\netcoreapp2.0"
dotnet "EncAndDecUtilCUICore.dll" > ..\..\..\..\ResultCore20.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the EncAndDecUtilCUICore(30).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\core30\bin\Debug\netcoreapp3.0"
dotnet "EncAndDecUtilCUICore.dll" > ..\..\..\..\ResultCore30.txt
cd %CURRENTDIR%

pause

rem -------------------------------------------------------
endlocal