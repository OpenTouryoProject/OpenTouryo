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
@rem Batch build of SimpleBatch.
@rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Tests\TestBatch\SimpleBatch\SimpleBatch.sln"
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\TestBatch\SimpleBatch\SimpleBatch.sln"

dotnet restore "Frameworks\Tests\TestBatch\SimpleBatchCore\SimpleBatchCore.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Tests\TestBatch\SimpleBatchCore\SimpleBatchCore.sln"

@echo --------------------------------------------------
@echo Test the SimpleBatch(48).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestBatch\SimpleBatch\bin\Debug"
SimpleBatch.exe /Dap SQL /Mode1 individual /Mode2 static /EXROLLBACK - > ..\..\..\ResultSimpleBatch48.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the SimpleBatchCore(80).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestBatch\SimpleBatchCore\bin\Debug\net8.0"
dotnet "SimpleBatchCore.dll" -- /Dap SQL /Mode1 individual /Mode2 static /EXROLLBACK - > ..\..\..\..\ResultSimpleBatchCore80.txt
cd %CURRENTDIR%

pause

rem -------------------------------------------------------
endlocal