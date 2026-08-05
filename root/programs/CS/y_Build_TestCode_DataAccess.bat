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
@rem Batch build of TestDataAccess.
@rem --------------------------------------------------
..\nuget.exe restore "Frameworks\Tests\TestDataAccess\TestDataAccessFx48.sln" %NUGET_MSBUILD%
%BUILDFILEPATH% %COMMANDLINE% "Frameworks\Tests\TestDataAccess\TestDataAccessFx48.sln"

dotnet restore "Frameworks\Tests\TestDataAccess\TestDataAccessCore100.sln"
dotnet msbuild %COMMANDLINE% "Frameworks\Tests\TestDataAccess\TestDataAccessCore100.sln"

@rem --------------------------------------------------
@rem /MODE SQLONLY : SQL Server only (the default).
@rem /MODE LOCAL   : all the DBMS running on the local Docker.
@rem The expected results (Result*.txt) are recorded with SQLONLY.
@rem --------------------------------------------------

@echo --------------------------------------------------
@echo Test the TestDataAccessFx(48).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestDataAccess\net48\bin\Debug"
"TestDataAccessFx.exe" /MODE SQLONLY > ..\..\..\Result48.txt
cd %CURRENTDIR%

@echo --------------------------------------------------
@echo Test the TestDataAccessCore(100).
@echo --------------------------------------------------
set CURRENTDIR=%cd%
cd "Frameworks\Tests\TestDataAccess\core100\bin\Debug\net10.0"
dotnet "TestDataAccessCore.dll" -- /MODE SQLONLY > ..\..\..\..\ResultCore100.txt
cd %CURRENTDIR%

pause

rem -------------------------------------------------------
endlocal
