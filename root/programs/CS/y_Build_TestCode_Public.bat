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
@rem Batch build of TestCode.
@rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Tests\TestCode\TestCodeFx45.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\TestCode\TestCodeFx45.sln"

..\nuget.exe restore "Frameworks\Tests\TestCode\TestCodeFx46.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\TestCode\TestCodeFx46.sln"

..\nuget.exe restore "Frameworks\Tests\TestCode\TestCodeFx47.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\TestCode\TestCodeFx47.sln"

..\nuget.exe restore "Frameworks\Tests\TestCode\TestCodeFx48.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\TestCode\TestCodeFx48.sln"

dotnet restore "Frameworks\Tests\TestCode\TestCodeCore20.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Tests\TestCode\TestCodeCore20.sln"

dotnet restore "Frameworks\Tests\TestCode\TestCodeCore30.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Tests\TestCode\TestCodeCore30.sln"

@echo --------------------------------------------------
@echo Test the TestCodeFx(45).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestCode\net45\bin\Debug"
"TestCodeFx.exe" > ..\..\..\Result45.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the TestCodeFx(46).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestCode\net46\bin\Debug"
"TestCodeFx.exe" > ..\..\..\Result46.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the TestCodeFx(47).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestCode\net47\bin\Debug"
"TestCodeFx.exe" > ..\..\..\Result47.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the TestCodeFx(48).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestCode\net48\bin\Debug"
"TestCodeFx.exe" > ..\..\..\Result48.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the TestCodeCore(20).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestCode\core20\bin\Debug\netcoreapp2.0"
dotnet "TestCodeCore.dll" > ..\..\..\..\ResultCore20.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the TestCodeCore(30).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestCode\core30\bin\Debug\netcoreapp3.0"
dotnet "TestCodeCore.dll" > ..\..\..\..\ResultCore30.txt
cd %CURRENTDIR%

pause

rem -------------------------------------------------------
endlocal