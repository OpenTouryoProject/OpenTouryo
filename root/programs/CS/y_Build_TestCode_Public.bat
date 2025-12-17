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
..\nuget.exe restore "Frameworks\Tests\TestCode\TestCodeFx48.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\TestCode\TestCodeFx48.sln"

dotnet restore "Frameworks\Tests\TestCode\TestCodeCore80.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Tests\TestCode\TestCodeCore80.sln"

@echo --------------------------------------------------
@echo Test the TestCodeFx(48).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestCode\net48\bin\Debug"
"TestCodeFx.exe" > ..\..\..\Result48.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the TestCodeCore(100).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestCode\core100\bin\Debug\net10.0"
dotnet "TestCodeCore.dll" > ..\..\..\..\ResultCore100.txt
cd %CURRENTDIR%

pause

rem -------------------------------------------------------
endlocal