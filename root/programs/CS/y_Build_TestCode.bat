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

dotnet restore "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUICore.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Tests\EncAndDecUtilCUI\EncAndDecUtilCUICore.sln"

set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\net45\bin\Debug"
"EncAndDecUtilCUIFx.exe" > Result45.txt
cd %CURRENTDIR%

set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\net46\bin\Debug"
"EncAndDecUtilCUIFx.exe" > Result46.txt
cd %CURRENTDIR%

set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\net47\bin\Debug"
"EncAndDecUtilCUIFx.exe" > Result47.txt
cd %CURRENTDIR%

set CURRENTDIR=%cd%
cd "Frameworks\Tests\EncAndDecUtilCUI\core20\bin\Debug\netcoreapp2.0"
dotnet "EncAndDecUtilCUICore.dll" > ResultCore.txt
cd %CURRENTDIR%

pause

rem -------------------------------------------------------
endlocal